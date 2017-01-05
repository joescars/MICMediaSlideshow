# MIC Media Player

UWP Application that displays a simple slide show from the images uploaded via the Media Manager application. This app was designed to run from a Raspberry Pi2 on a TV @ 1920x1080 resolution but you can adapat it accordingly.  

## Requirements

* Windows Universal Platform (UWP)
* Windows 10 IoT Core (if deploying to Pi)

## Instructions

* Create a file named **settings.json** in the root. 
* This is the file that will contain your configuration settings. Included in the project is a **settings.example.json** for reference. 
* **NOTE:** after creating this file you must view properties and set "*Build Action*" to "*Content*"

## Configuration Items

1. **MediaPlayerAPIUrl** - URL to your api from the MIC Media Manager
2. **SlideInterval** - Duration in SECONDS in between slide transition
3. **PollInterval** - Duration in MINUTES for polling the database for new slides. 

Once these items are configured your application will run. 

