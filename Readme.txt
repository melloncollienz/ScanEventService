The general gist of the feedback I got was to do the things that i listed in the last readme
I've still assumed that it's supposed to be a windows service based the feedback i got.  If you wanted it to be a web/something that lives online, you'd need to co-ordidate the last event ids better between threads/containers/instances depending on the infrastruture

Things that are still outstanding:
Registration in Autofac for NLog and the NLog config file
The rest of the unit tests
There should be a shim application to loads Scan Event Library, but this may not be necessary if you want that covered by unit tests
There's some code tidy ups that should be done.
I've still made it a file save/load because i don't want to add a docker container for the db
Install/uninstall commands for the windows service, but this should actually be a signed installer.


