var express = require('express');
var app = express();
var readline = require('readline');
var fs = require('fs');
var Promise = require('promise');

function findHwAddr(ip) {
	return new Promise(function(fulfill, reject) {
            var hwaddr = "unknown";
            var lineReader = readline.createInterface({                  
		input: fs.createReadStream('/proc/net/arp'),
		terminal: false
            });
            lineReader.on('line', function (line) {
		var splitted = line.split(/\s+/);
		if (splitted[0] === ip) {
			hwaddr = splitted[3];
			fulfill(hwaddr);
		}
            });
            lineReader.on('close', function(a) {
		   fulfill(hwaddr);
            });
	});
}

app.get('/', function (req, res) {
  console.log("Got a request from "+ req.connection.remoteAddress);
  
  res.setHeader('Content-Type', 'application/json')
  findHwAddr(req.connection.remoteAddress).then(function(hwaddr) {
    res.send(JSON.stringify({
      'ip': req.connection.remoteAddress,
      'hwaddr': hwaddr,
    }, null, 4));
  });
});
  
var port = 8000;
if (process.argv.length > 2)
  port = parseInt(process.argv[2])

app.listen(port, function () {
    console.log('Example app listening on port ' + port);
});
