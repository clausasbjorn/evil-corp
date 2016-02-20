#### Getting Started ####
The Evil API is where we ingest event data from our beacons.

The API is based on Azure Service Fabric. In order to get started using Service Fabric we suggest you read the blog post series available here [http://blog.geist.no/azure-service-fabric-introduction-getting-it-running/](http://blog.geist.no/azure-service-fabric-introduction-getting-it-running/).

When you are ready to get going, clone the application and open it in Visual Studio 2015.

#### The projects ####

The project consists of these parts.

* EvilCorp, the Service Fabric project containing configuration of the services
* EvilCorp.Ingest, the API endpoint that receives tracking, user and calibration data
* EvilCorp.Common, commonly used code for Service Fabric actors
* EvilCorp.People, the Actors for people tracking and analytics
* EvilCorp.People.Interface, the contracts and factories required to access People-actors from the API

#### Adding additional API endpoints ####
All endpoints are found in TrackerController.fs. The API is an ordinary WebApi application, which you can add additional actions to.

#### Extending People actors ####
To extend the People actors

* Add a new contract in the interfaces library
* Add a new method to the IPerson interface
* Implement the new method in the Person-type in the Actors library

Notice that the behaviour of the Person object is not part of the type itself, but specified in a module alongside it be more F# / functional idiomatic.

