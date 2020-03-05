var AWS = require('aws-sdk');

AWS.config.update({region: 'REGIONHERE'});

var params = {
    Message: '{ \"OrderId\":\"483f2ce1-06f4-4774-9630-e7c31328ba66\", \"CustomerTelephoneNumber\":\"123456789\"}', 
    TopicArn: 'arn:aws:sns:REGIONHERE:ACCOUNTNUMBERHERE:orderunacknowledged'
};

var publishTextPromise = new AWS.SNS({apiVersion: '2010-03-31'})
.publish(params)
.promise();

// Handle promise's fulfilled/rejected states
publishTextPromise.then(
  function(data) {
    console.log(`Message ${params.Message} send sent to the topic ${params.TopicArn}`);
    console.log("MessageID is " + data.MessageId);
  }).catch(
    function(err) {
    console.error(err, err.stack);
  });
  