## Spectero Command Proxy

This is the component responsible for proxying commands on behalf of users using the Cloud app to the actual daemon.

It was primarily created to get rid of mixed-mode content issues, but offers many other benefits as well.

Eventually, node ("daemon") verification will be entirely offloaded to this component as well (to hide the origin IP of the cloud system.)