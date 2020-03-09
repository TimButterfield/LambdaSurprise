# LambdaSurprise

What happens when you have use a background thread in an AWS lambda? 

This repository was created as part of a [blog post](https://blog.timbutterfield.co.uk/2020/02/11/serverless-deploy-developer-policy/) about using a third party SDK without giving consideration to whether the package in question was fit for my execution model. Hours of confusion ensued as a conseqeunce of not giving thought to that question, wanting to iterate quickly and considering the code from the SDK I was about to use.

#### TLDR

Background threads shouldn't be used in AWS lambdas, since "**_the background thread will be frozen once a Lambda event is processed and may not ever be unfrozen if more Lambda events are not received for some time._**"

The above quote was taken from the [aws logging dotnet project](https://github.com/aws/aws-logging-dotnet)


![See blog post for what happens when you use background tasks](https://github.com/TimButterfield/lambdasurprise/AWSLambdaLogs-Threading.png)

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
1) Run **_npm run deploy_** from {project_root}/src/LambdaSurprise.Services directory. 

There are a multitude of reasons why deploying a Lambda to AWS can fail.
If it turns out to be a policy/permissions issue, [this](https://blog.timbutterfield.co.uk/2020/02/11/serverless-deploy-developer-policy/) may help

####### Triggering the lambda

There are two Lambda triggers. Firstly there's an [sns trigger](https://github.com/TimButterfield/LambdaSurprise/blob/master/src/LambdaSurprise.Services/serverless.yml#L18) and secondly a [cron trigger](https://github.com/TimButterfield/LambdaSurprise/blob/master/src/LambdaSurprise.Services/serverless.yml#L19). Be sure that you disable the [scheduled trigger](https://docs.aws.amazon.com/eventbridge/latest/userguide/run-lambda-schedule.html) in the AWS console once you've managed to replicate the scenario.

I'd recommend that you trigger the Lambda 10-20 times by running the publish-to-sns.js node script repeatedly, and then leave it for a while (I think I left it > 30 minutes before re-triggering). 

BEFORE running the script though, you'll need to edit the script, replacing the "**_REGIONHERE_**" placeholder and the "**_ACCOUNTNUMBERHERE_**" placeholder as per your account and region details.