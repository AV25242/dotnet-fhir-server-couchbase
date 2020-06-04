This is a how-to-guide for using fhir-server using dotnet core 3.1, just follow along the steps and you should be just fine.

Step 1 (pre-requisites) : 
      
      Install dotnet core 3.1 by following the link https://dotnet.microsoft.com/download/dotnet-core/3.1, you can skip this step if dotnet core 3.1 is already installed.
      
      Download and Install git from https://git-scm.com/downloads
      
      Open a terminal window / command prompt depending on the operating system and navigate to your development folder.
      


Step 2 (download, build and run application code): 

         git clone https://github.com/AV25242/dotnet-fhir-server-couchbase.git
         Navigate to the folder dotnet-fhir-server-couchbase by typing 
                   cd dotnet-fhir-server-couchbase
         
         Next, locate and open appSettings.json in text editor of your choice.
         Modify this section of this file to point to your couchbase server configuration that is right for your settings and          save the file.
         
         "CouchbaseServerConfig": {
              "Host": "http://172.23.121.9",
              "Bucket": "fhir_admin",
              "Username": "Administrator",
              "Password": "password"
          }
          
          for instance if your couchbase server runs on a localhost then the configuration section should look like this
          
          "CouchbaseServerConfig": {
              "Host": "http://localhost",
              "Bucket": "fhir_admin",
              "Username": "Administrator",
              "Password": "password"
          }
          
          Next, on the terminal type
          
          dotnet build
          
                you should see
          
                    Build succeeded.
                    0 Warning(s)
                    0 Error(s)

         Next, on the terminal type
         
         dotnet run
         
                you should see
                
                Now listening on: http://localhost:5000
                Application started. Press Ctrl+C to shut down.
                
You are all set, application is now running on your local machine on port 5000

Step 3 ( Fun with CURL - API Readiness) : 

  Search Patient by Id

         curl -v "http://localhost:5000/FHIR/api/Patient?id=e3ae7831-14c8-4d13-a8fb-dad68fa12bc9"
         
  Search Patient by Id as Field

         curl -v http://localhost:5000/FHIR/api/Patient/e3ae7831-14c8-4d13-a8fb-dad68fa12bc9
         
 
  Search Patient by Email
  
         curl -v http://localhost:5000/FHIR/api/Practitioner/?email=Quinton758.Hammes673@example.com

