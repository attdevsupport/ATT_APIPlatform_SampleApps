require 'bundler'

#make sure all gems are installed
Bundler.require

#require the file for ruby 1.9+
require_relative './speech'

run Speech
