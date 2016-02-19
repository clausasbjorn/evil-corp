# Evil Helper
This lovly helper API gives back the ip address and the hardware of the
connecting api. It is very helpful... listens on port 80 by default.


### Installing dependencies
```
npm install
```

### To run it

You may specify a port like 80, defaults to 8000.

```
node app.js 80
```

### To query it
```
curl http://yourip/
```

A typical response would be:

```json
{
	'ip': '10.59.9.0',
	'hwaddr': 'b8:27:eb:38:0a:7e'
}
```
