InProcThrottle
==============

A framework to handle CPU throttling in a process e.g. in a process, between app domains or between processes

##Background
In some scenarios there might be a need to set an ceiling value for CPU for when it is ok to start a new job or similar. This can get quite tricky if you have multiple app domains or different processes.

The idea is to use a manager that sets the threshold values and one or more clients that can consume that information.

The Manager set's up a ceiling value, e.g. it's only ok to do anything if the CPU is less than X.

The Clients asks before they do anything that is concidered to be something that might consume CPU, e.g. a heavy Job.

##Setup
```java
//Manager side
ThrottleManager.Config<InterProcessProvider>("JobScope", 0, 360);

//Client side - check every time manually
ThrottleClient.CanIRun<InterProcessProvider>("JobScope");

//Client side - Run it if it's allowed now execute a callback when it's ok to run the job
ThrottleClient.CanIRun<InterProcessProvider>("Test", () => { blnEnd = true; });
```

##Communication providers
There exists two communication providers, one for inter process/app domain communication and one for intra appdomain communication.

###InterProcessProvider
This provider uses MemoryMappedFiles to communicate between the manager and the clients. No disk access is needed, only writes to memory.

###SharedInProcessProvider
A static memory provider for use within an app domain.


##Examples
- See unit tests for basic examples
- Check the example project for an example with different app domains controlled with one Manager
