# LambdaSurprise

What happens when you use a background thread in an AWS lambda? 

This repository was created as part of a [blog post](https://blog.timbutterfield.co.uk/2020/02/11/serverless-deploy-developer-policy/) about using Open Source Software without giving consideration to whether it is fit for the execution model. In my case, hours of confusion ensued as a consequence of wanting to iterate quickly, not reading the SDK source code and not giving thought to whether the SDK was right for use in an AWS Lambda. 

#### TLDR

Background threads shouldn't be used in AWS lambdas, since "**_the background thread will be frozen once a Lambda event is processed and may not ever be unfrozen if more Lambda events are not received for some time._**"

The above quote was taken from the [aws logging dotnet project](https://github.com/aws/aws-logging-dotnet)


![See blog post for what happens when you use background tasks](https://github.com/TimButterfield/lambdasurprise/blob/master/AWSLambdaLogs-Threading.png)

Fig 1.0 **_An extract from cloud watch logs showing how log statements from one Lambda execution appear with the log statements from a subsequent execution. Caused by background threads being frozen, as described above_**.

In my case, the real world problem that occurred in a production system was caused by this [one line of code](https://github.com/optimizely/csharp-sdk/blob/master/OptimizelySDK/Event/Dispatcher/HttpClientEventDispatcher45.cs#L78). Thankfully, the implications were not significant, but they could be subject to the system you're building.  

#### Give it a try
If you have any desire to pull down the code, launch and then execute an AWS Lambda to see what happens, follow these steps. 

###### To compile and package: 
1) Clone/Fork the repository
2) Install the [.net core 2.1 sdk](https://dotnet.microsoft.com/download/dotnet-core/2.1) (if not already installed)
3) Install the [Serverless framework](https://serverless.com/framework/docs/getting-started/) (if not already installed)
4) Install [node](https://nodejs.org/en/download/) and npm (if not already installed)
5) Run **_npm run build_** from {project_root}/src/LambdaSurprise.Services directory
6) Run **_npm run publish_** from {project_root}/src/LambdaSurprise.Services directory
7) Run **_npm run package_** from {project_root}/src/LambdaSurprise.Services directory

###### To deploy to AWS: 
1) Install the [AWS sdk for javascript](https://www.npmjs.com/package/aws-sdk)
2) Run **_npm run deploy_** from {project_root}/src/LambdaSurprise.Services directory. 

There are a multitude of reasons why deploying a Lambda to AWS can fail.
If you do experience problems, which turn out to be a policy/permissions issue, [this](https://blog.timbutterfield.co.uk/2020/02/11/serverless-deploy-developer-policy/) may help

###### Triggering the Lambda

There are two Lambda triggers. Firstly there's an [sns trigger](https://github.com/TimButterfield/LambdaSurprise/blob/master/src/LambdaSurprise.Services/serverless.yml#L18) and secondly a [cron trigger](https://github.com/TimButterfield/LambdaSurprise/blob/master/src/LambdaSurprise.Services/serverless.yml#L19). Be sure that you disable the [scheduled trigger](https://docs.aws.amazon.com/eventbridge/latest/userguide/run-lambda-schedule.html) in the AWS console once you've managed to replicate the scenario.

I'd recommend that you trigger the Lambda 10-20 times by running the publish-to-sns.js node script repeatedly, and then leave it for a while (I think I left it > 30 minutes before re-triggering). After a period > 30 minutes, repeat the re-triggering of the Lambda. Hopefully you'll see evidence from the CloudWatch logs of the above. 

BEFORE running the script though, you'll need to edit the script, replacing the "**_REGIONHERE_**" placeholder and the "**_ACCOUNTNUMBERHERE_**" placeholder as per your account and region details.
