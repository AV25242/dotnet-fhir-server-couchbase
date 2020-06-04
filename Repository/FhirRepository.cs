using System;
using System.Collections.Generic;
using Couchbase.Core;
using Couchbase.Configuration.Client;
using Couchbase.Authentication;
using Couchbase;
using Couchbase.N1QL;
using System.Threading.Tasks;
using System.Linq;

namespace FHIR.Repository
{
    public class FhirRepository
    {
        private readonly IBucket _bucket;

        static List<string> Types = new List<string>()
        {
            "AllergyIntolerance"
            ,"CarePlan"
            ,"CareTeam"
            ,"Claim"
            ,"Condition"
            ,"Device"
            ,"DiagnosticReport"
            ,"Encounter"
            ,"ExplanationOfBenefit"
            ,"Goal"
            ,"ImagingStudy"
            ,"Immunization"
            ,"MedicationAdministration"
            ,"MedicationRequest"
            ,"Observation"
            ,"Organization"
            ,"Patient"
            ,"Practitioner"
            ,"Procedure"

        };

        static Dictionary<string, string> Eval = new Dictionary<string, string>()
        {
            { "lt","<" },
            { "gt",">" },
            { "ne","!=" },
            { "ge",">=" },
            { "le","<=" }

        };

        static Dictionary<string, string> ItemArray = new Dictionary<string, string>()
        {
            {"phone" ," AND ANY v in {type}.telecom SATISFIES (v.`system`='phone' AND v.`value`=$phone) END " },
            {"email" ," AND ANY v in {type}.telecom SATISFIES (v.`system`='email' AND v.`value`=$email) END " },
            { "_text", " AND SEARCH( {type},$_text) "},
            { "name", " AND ARRAY_COUNT({type}.name) > 0 AND (ANY x in ARRAY_STAR({type}.name).family SATISFIES x like $name END OR  ANY y in array_flatten(array_star({type}.name).given,1) SATISFIES y like $name END ) " },
            { "identifier"," AND ANY v in {type}.identifier SATISFIES CONCAT(v.`system`,'|',v.`value`)= $identifier END " },
            { "code"," AND any v in {type}.code.coding SATISFIES CONCAT(v.`system`,'|',v.`code`)= $code END" }
        };

        public FhirRepository(Models.ICouchbaserServerConfiguration couchbaseConfig)
        {
            ClusterHelper.Initialize(new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(couchbaseConfig.Host) },
                EnableTcpKeepAlives = true,
                TcpKeepAliveInterval = 1000,
                TcpKeepAliveTime = 300000
            }, new PasswordAuthenticator(couchbaseConfig.Username, couchbaseConfig.Password));



            _bucket = ClusterHelper.GetBucket(couchbaseConfig.Bucket);

         
        }

        #region Read(s) 


        public async Task<dynamic> GetFhir(string type, Dictionary<string,string> predicates)
        {
            var query = "SELECT {0} FROM `" + _bucket.Name + "` {0} {1} WHERE {0}.resourceType = $type ";

            var join = " INNER JOIN `" + _bucket.Name + "` {child} ON split({child}.subject.reference,':')[2] = {0}.id AND {child}.resourceType= $child ";

            var queryRequest = new QueryRequest()
             .AddNamedParameter("$type", type)
             .Metrics(false);

            var modifiedPredicates = new Dictionary<string,string>();

            query = query.Replace("{0}", "`" + type.ToLower() + "`");

            bool added = false;
            foreach (var item in predicates)
            {
                var key = item.Key.Replace(":", ".");
                var keys = key.Split(".");

                if(keys.Length > 1)
                {
                    if (Types.Contains(keys[0]))
                    {
                        if (!added)
                        {
                            join = join.Replace("{child}", "`" + keys[0].ToLower() +"`");
                            join = join.Replace("{0}", "`" + type.ToLower() + "`");

                            query = query.Replace("{1}", join);
                            queryRequest.AddNamedParameter("$child", keys[0]);
                        }

                        key = key.Replace(keys[0], keys[0].ToLower());

                        added = true;
                    }
                    else
                    {
                        key = type.ToLower() + "." + key;
                    }
                }
                else
                {
                    key = type.ToLower() + "." + key;
                }

                modifiedPredicates.Add(key, item.Value);
                
            }

            query = query.Replace("{1}", "");
            foreach (var item in modifiedPredicates)
            {
                var keys = item.Key.Split(".");

                var key = item.Key.Replace(".", "`.`");

                if (key.Contains('.'))
                {
                    key = key.Insert(0, "`");
                    key = key.Insert(key.Length, "`");
                }
                var itemKey = item.Key.Replace(":", "A").Replace(".","A");

                var val = item.Value;
                var op = "=";


                if (!string.IsNullOrEmpty(val))
                {
                    if (val.Length > 2)
                    {
                        var str = val.Substring(0, 2);
                        if (Eval.ContainsKey(str))
                        {
                            op = Eval[str];
                            val = val.Substring(2, val.Length-2);
                        }
                    }
                }

                
             
                if (keys.LastOrDefault<string>() == keys.FirstOrDefault<string>(x => ItemArray.ContainsKey(x)))
                {
                    
                    var ik = keys.First<string>(x => ItemArray.ContainsKey(x));

                    query = query + ItemArray[ik];
                    query = query.Replace("{type}", keys[0].ToLower());
                    queryRequest.AddNamedParameter("$" + ik, (ik=="name") ? val+"%" : val);

                }
                else if (float.TryParse(val, out float fValue))
                {
                    query = query + " AND " + key + op +  "$" + itemKey;
                    queryRequest.AddNamedParameter("$" + itemKey, fValue);
                }
                else if (Int32.TryParse(val, out int iValue))
                {
                    query = query + " AND " + key + op + "$" + itemKey;
                    queryRequest.AddNamedParameter("$" + itemKey, iValue);
                }
                else if (DateTime.TryParse(val, out DateTime dValue))
                {
                    query = query + " AND " + key + op + "$" + itemKey;
                    queryRequest.AddNamedParameter("$" + itemKey, dValue);
                }
                else 
                {
                    query = query + " AND " + key + op + "$" + itemKey;
                    queryRequest.AddNamedParameter("$" + itemKey, val );
                }
            }
          
            query = query + " LIMIT 10;";

            
            
            
            queryRequest.Statement(query);

            var result = await _bucket.QueryAsync<dynamic>(queryRequest);

            return result;
        }

        #endregion

       
    }
}
