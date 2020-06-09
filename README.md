Setup the Couchbase FHIR server consists of two parts. 

    i) Install Couchbase server and load the Synthea data.
    ii) Deploy the dotnet fhir server code.

1. Setup the Couchbase server and Synthea data

a)  Install couchbase : https://www.couchbase.com/downloads?family=couchbase-server
    Enable Data, Index, Search, and Query Services.
    
b)  Create two buckets, synthea, fhir_admin. Allocate 300MB to each bucket.

c)  Download the synthea data to a local drive: https://storage.googleapis.com/synthea-public/synthea_sample_data_fhir_r4_sep2019.zip

d)  Load the data into the Couchbase synthea bucket.

     >cbimport json -c couchbase://<couchbase-server> -u Administrator -p password -b synthea 
              -d file://Downloads/synthea_sample_data_fhir_r4_sep2019.zip  -f sample -g key::#UUID# -t 4
    
e)  Connect to couchbase with cbq and set a long timeout:  /opt/couchbase/bin/cbq -e http://localhost:8091 
              -u=Administrator  u-timeout="30m"
              
f)  Execute the following two N1QL statements:

      cbq> CREATE PRIMARY INDEX ON synthea;
      cbq> UPSERT INTO fhir_admin (key UUID(),value doc_body)
              SELECT  l.resource doc_body FROM synthea s UNNEST s.entry l
              WHERE l.resource.id IS NOT MISSING
              
g)  Open the fhir_gsi.txt, and excute all the N1QL create index in the file.

h)  Open the fhir_fts.txt and run the CURL command to create the FTS Indexes.

2. Deploy the dotnet fhir server code

a) Install dotnet core 3.1 by following the link https://dotnet.microsoft.com/download/dotnet-core/3.1, 
      you can skip this step if dotnet core 3.1 is already installed.
      
b) Download and Install git from https://git-scm.com/downloads

c) Open a terminal window / command prompt depending on the operating system and navigate to your development folder.

      >git clone https://github.com/AV25242/dotnet-fhir-server-couchbase.git
   
d) Navigate to the folder dotnet-fhir-server-couchbase by typing 

      >cd dotnet-fhir-server-couchbase
   
e) Next, locate and open appSettings.json in text editor of your choice.
   Modify this section of this file to point to your couchbase server configuration that is right for your settings and          save the file.
         
         "CouchbaseServerConfig": {
              "Host": "http://<couchbase-server>",
              "Bucket": "fhir_admin",
              "Username": "Administrator",
              "Password": "password"
          }
          
f) Next, on the terminal type

      >dotnet build      
   you should see    
   
                    Build succeeded.
                    0 Warning(s)
                    0 Error(s)

g) Next, on the terminal type

      >dotnet run     
   you should see            
                Now listening on: http://localhost:5000
                Application started. Press Ctrl+C to shut down.
                
You are all set, application is now running on your local machine on port 5000

3) Verify that everything is working ( Fun with CURL - API Readiness) 

a) Search Patient by Id as Field

       >curl -v http://localhost:5000/FHIR/api/Patient/e3ae7831-14c8-4d13-a8fb-dad68fa12bc9
  you shoud see the patient document      
  
