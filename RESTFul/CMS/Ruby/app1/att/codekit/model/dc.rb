require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      class Capabilities
        attr_reader :name, :vendor, :model, :firmware_version, :uaprof, :mms_capable, 
          :assisted_gps, :location_technology, :device_browser, :wap_push_capable

        # Create a Capabilities object
        #
        # @param opts [Hash] Optional parameters to set
        #
        # @option opts [String] :name The name of the device
        # @option opts [String] :vendor The name of the vendor
        # @option opts [String] :model The name of the model
        # @option opts [String] :firmware_version The model's firmware version
        # @option opts [String] :uaprof The uaprof of the device
        # @option opts [String] :mms_capable The mms capabilities of the device
        # @option opts [String] :assisted_gps If the device is capable of assisted gps
        # @option opts [String] :location_technology The location support of the device
        # @option opts [String] :device_browser The device's supported browser
        # @option opts [String] :wap_push_capable If the device is capable of wap
        def initialize(opts = {})
          @name = (opts[:name] || 'Unknown')
          @vendor = (opts[:vendor] || 'Unknown')
          @model = (opts[:model] || 'Unknown')
          @firmware_version = (opts[:firmware_version] || 'Unknown')
          @uaprof = (opts[:uaprof] || 'Unknown')
          @mms_capable = (opts[:mms_capable] || 'Unknown')
          @assisted_gps = (opts[:assisted_gps] || 'Unknown')
          @location_technology = (opts[:location_technology] || 'Unknown')
          @device_browser = (opts[:device_browser] || 'Unknown')
          @wap_push_capable = (opts[:wap_push_capable] || 'Unknown')
        end

        # Defined iterator for easily looping over key/values 
        #
        # @yield [key, value] Gives two Strings, the name and value of the capability
        def each
          yield "Name", @name
          yield "Vendor", @vendor
          yield "Model", @model
          yield "Firmware Version", @firmware_version
          yield "UaProf", @uaprof
          yield "Mms Capable", @mms_capable
          yield "Assisted Gps", @assisted_gps
          yield "Location Technology", @location_technology
          yield "Device Browser", @device_browser
          yield "Wap Push Capable", @wap_push_capable
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Capabilities] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a json string that has been decoded
        #
        # @return [Capabilities] a parsed object
        def self.createFromParsedJson(json)
          name = json['Name']
          vendor = json['Vendor']
          model = json['Model']
          firmware_version = json['FirmwareVersion']
          uaprof = json['UaProf']
          mms_capable = json['MmsCapable']
          assisted_gps = json['AssistedGps']
          location_technology = json['LocationTechnology']
          device_browser = json['DeviceBrowser']
          wap_push_capable = json['WapPushCapable']

          new(:name => name,
              :vendor => vendor,
              :model => model,
              :firmware_version => firmware_version,
              :uaprof => uaprof,
              :mms_capable => mms_capable,
              :assisted_gps => assisted_gps,
              :location_technology => location_technology,
              :device_browser => device_browser,
              :wap_push_capable => wap_push_capable)
        end
      end

      class DeviceID
        attr_reader :type_allocation_code

        # Create's a device id
        #
        # @param type_allocation_code [#to_s] the code returned from the api (default: Unknown)
        def initialize(type_allocation_code)
          @type_allocation_code = (type_allocation_code || 'Unknown')
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [DeviceID] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [DeviceID] a parsed object
        def self.createFromParsedJson(json)
          new(json['TypeAllocationCode'])
        end
      end

      class DCResponse < ImmutableStruct.new(:device_id, :capabilities)
        # Holds the response of a DC api call
        #
        # @!attribute [r] device_id
        #   @return [String] id of the device
        # @!attribute [r] capabilities 
        #   @return [#Capabilities] the devices supported capabilities

        # Factory method to create an object from a json string
        #
        # @param json [Object] a json encoded string
        #
        # @return [DCResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [DCResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json['DeviceInfo']

          capabilities = Capabilities.createFromParsedJson(root['Capabilities'])
          device_id = DeviceID.createFromParsedJson(root['DeviceId'])

          new(device_id, capabilities)
        end
      end

    end
  end
end
