require 'bundler'

#make sure all gems are installed
Bundler.require

begin
  #require the file for ruby 1.9+
  require_relative './mms'
rescue NoMethodError 
  #if error then we're in ruby 1.8
  require './mms'
end

run Sinatra::Application
