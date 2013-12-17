require "bundler/gem_tasks"
require 'yard'
require 'rake/testtask'
require 'rake/clean'

CLEAN.include("pkg/*.gem")

task :default => [:clean, :build]

YARD::Rake::YardocTask.new do |t|
    t.files   = ['lib/**/*.rb']   
end

Rake::TestTask.new do |t|
  t.libs = ["lib"]
  t.warning = true
  t.verbose = true
  t.test_files = FileList['test/test_*.rb']
end
