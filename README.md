# LambdaSurprise

What happens when you have use a background thread in an AWS lambda? 

This repository was created as part of a [blog post](https://blog.timbutterfield.co.uk/2020/02/11/serverless-deploy-developer-policy/) about using a third party SDK without giving consideration to whether the package in question was fit for my execution model. 

###TLDR

Background threads shouldn't be used in AWS lambdas, since *the background thread will be frozen once a Lambda event is processed and may not ever be unfrozen if more Lambda events are not received for some time.*