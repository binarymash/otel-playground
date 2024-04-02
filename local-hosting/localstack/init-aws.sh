#!/bin/bash
awslocal dynamodb create-table --table-name RollHistory \
    --attribute-definitions AttributeName=Date,AttributeType=S AttributeName=Player,AttributeType=S \
    --key-schema AttributeName=Date,KeyType=HASH AttributeName=Player,KeyType=RANGE \
    --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5