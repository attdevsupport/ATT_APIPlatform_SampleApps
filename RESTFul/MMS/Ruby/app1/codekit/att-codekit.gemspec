# coding: utf-8
lib = File.expand_path('../lib', __FILE__)
$LOAD_PATH.unshift(lib) unless $LOAD_PATH.include?(lib)
require 'att/codekit/version'

Gem::Specification.new do |spec|
  spec.name          = "att-codekit"
  spec.version       = Att::Codekit::VERSION
  spec.authors       = ["kh455g"]
  spec.description   = %q{Standardized interface for making calls to ATT's cloud API services}
  spec.summary       = %q{Codekit for accessing AT&T's cloud services}
  spec.homepage      = "http://developer.att.com"
  spec.license       = "AT&T: Software Development Kit Tools Agreement"
  spec.has_rdoc      = true

  spec.files         = Dir['lib/**/*', 'examples/**/*']
  spec.files         += Dir['*.gemspec', 'Gemfile', 'Rakefile', 'README.md']
  spec.executables   = spec.files.grep(%r{^bin/}) { |f| File.basename(f) }
  spec.test_files    = spec.files.grep(%r{^(test|spec|features)/})
  spec.require_paths = ["lib"]

  #install dependencies
  spec.add_development_dependency "rake"
  spec.add_development_dependency "yard"
  spec.add_development_dependency "bundler", "~> 1.3"

  #runtime dependencies
  spec.add_runtime_dependency "json"
  spec.add_runtime_dependency "immutable_struct"
  spec.add_runtime_dependency "rest-client", "~> 1.7"
end
