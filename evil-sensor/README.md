# Evil sensor

Evil sensor listens for wlan wireless beacons formats them and sends them to
the evil api.

## Requirements
* Linux OS / Raspberry pi
* Wireless dongles that support monitor mode

## Dependencies
* tcpdump
* scapy

On Raspberry Pi with raspbian:
```sh
$ sudo apt-get install tcpdump scapy
```

## Do a test run

Just start the evil-sensor.py directly on the command line, you will need to
have raw access to the wireless.

```
sudo python evil-sensor.py wlan0
```

## Start on reboot

For each interface you can repeat the enable line.

```sh
# sudo cp evil-sensor@.service /etc/systemd/system
# sudo systemctl enable evil-sensor@wlan0
```

We run wlan0 on channel 1, wlan1 on channel 6 and wlan2 on channel 11 to
cover most channels.

