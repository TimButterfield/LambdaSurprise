# LambdaSurprise

What happens when you have use a background thread in an AWS lambda? 

This repository was created as part of a [blog post](https://blog.timbutterfield.co.uk/2020/02/11/serverless-deploy-developer-policy/) about using a third party SDK without giving consideration to whether the package in question was fit for my execution model. Hours of confusion ensued as a conseqeunce of not giving thought to that question, wanting to iterate quickly and considering the code from the SDK I was about to use.

#### TLDR

Background threads shouldn't be used in AWS lambdas, since "**_the background thread will be frozen once a Lambda event is processed and may not ever be unfrozen if more Lambda events are not received for some time._**"


#### Download and Deploy
If you have any desire to pull down the code and launch an AWS Lambda to see what happens, follow these steps. 

1) Clone/Fork the repository
2) Install the [.net core 2.1 sdk](https://dotnet.microsoft.com/download/dotnet-core/2.1) (if not already installed)
3) Install the [Serverless framework](https://serverless.com/framework/docs/getting-started/) (if not already installed)
4) Install [node](https://nodejs.org/en/download/) and npm (if not already installed)
5) Run *npm run build* from the projects {project_root}/src/LambdaSurprise.Services directory