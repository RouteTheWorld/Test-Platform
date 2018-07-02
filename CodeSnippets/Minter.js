// Embedded within a Twilio "Function" so may require aditional requires to compile outside that ecosystem

exports.handler = function(context, event, callback) {
	const AccessToken = require('twilio').jwt.AccessToken;
    const VideoGrant = AccessToken.VideoGrant;
	
    // Used when generating any kind of tokens
    const twilioAccountSid = 'XXX';
    const twilioApiKey = 'XXX';
    const twilioApiSecret = 'XXX';
    
    const identity = 'user' + event.userID;
    
    console.log(identity);
    
    // Create Video Grant
    const videoGrant = new VideoGrant({
      room: event.roomID,
    });
    
    // Create an access token which we will sign and return to the client,
    // containing the grant we just created
    const token = new AccessToken(twilioAccountSid, twilioApiKey, twilioApiSecret);
    token.addGrant(videoGrant);
    token.identity = identity;
    
    // Serialize the token to a JWT string
    const JWTString = token.toJwt();
    
    console.log(JWTString);
    
    const JWTInJSON = JSON.stringify({ key: JWTString });
    
    console.log(JWTInJSON);

	callback(null, JWTInJSON);
};
