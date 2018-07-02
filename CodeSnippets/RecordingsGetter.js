
// Embedded within a Twilio "Function" so may require aditional requires to compile outside that ecosystem

exports.handler = function(context, event, callback) {
const Twilio = require("twilio");
const request = require("request");

const apiKeySid = "XXX";
const apiKeySecret = "XXX";
const accountSid = "XXX";
const client = new Twilio(apiKeySid, apiKeySecret, { accountSid: accountSid });

var recoridingsIDArray = [];
var recoringRawMedia = [];

console.log(event.roomID);

// Async function to get the URL of the specified recording resource
async function getMedia(recordingSid)
{
    console.log("start one" + recordingSid);
    
    const uri = `https://video.twilio.com/v1/Recordings/${recordingSid}/Media`;
    await client.request({ method: "GET", uri: uri }).then(response => {
          console.log("response");
          const mediaLocation = JSON.parse(response.body).redirect_to;
          console.log(mediaLocation);
          recoringRawMedia.push(mediaLocation);
          console.log("mediaLocation" + mediaLocation);
          console.log("recoringRawMedia" + recoringRawMedia);
          Promise.resolve();
        });
}

// Async function to get all the URLs from an array of recording resources
async function getAllMedia(recordingIDArray) {
    for (const item of recordingIDArray){
        await getMedia(item);
    }
    console.log("done");
    console.log(recoringRawMedia);
    callback(null, recoringRawMedia);
}

var options = {};
// Create a function that we want to be called when Twilio finishes fetching all recordings associated with a room
options.done = function done() {
    console.log("doneCalled " + recoridingsIDArray);
    
   getAllMedia(recoridingsIDArray);
};

// Create a function that we want to be called as Twilio returns each recording associated with the room
options.callback = function processEach(recordings) {
    recoridingsIDArray.push(recordings.sid);
};

// Query the room with the two functions we just set up
client.video.rooms(event.roomID)
            .recordings
            .each(options);
};
