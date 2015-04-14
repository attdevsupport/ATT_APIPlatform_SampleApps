require 'bundler'

#make sure all gems are installed
Bundler.require
require_relative './iam'

run IAM
