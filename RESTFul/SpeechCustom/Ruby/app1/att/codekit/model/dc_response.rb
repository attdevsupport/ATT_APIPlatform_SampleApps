
module Att
  module Codekit
    module Model

      class Capabilities
        attr_reader :name, :vendor, :model, :firmware_version, :uaprof, :mms_capable, 
          :assisted_gps, :location_technology, :device_browser, :wap_push_capable

        # Create a Capabilities object
        #
        # If any paramameter is nil then it will be set to 'Unknown'
        def initialize(name, vendor, model, firmware_version, uaprof, mms_capable,
                       assisted_gps, location_technology, device_browser, wap_push_capable)
          @name = (name || 'Unknown')
          @vendor = (vendor || 'Unknown')
          @model = (model || 'Unknown')
          @firmware_version = (firmware_version || 'Unknown')
          @uaprof = (uaprof || 'Unknown')
          @mms_capable = (mms_capable || 'Unknown')
          @assisted_gps = (assisted_gps || 'Unknown')
          @location_technology = (location_technology || 'Unknown')
          @device_browser = (device_browser || 'Unknown')
          @wap_push_capable = (wap_push_capable || 'Unknown')
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
      end

      class DeviceID
        attr_reader :type_allocation_code

        # Create's a device id
        #
        # @param type_allocation_code [#to_s] the code returned from the api (default: Unknown)
        def initialize(type_allocation_code)
          @type_allocation_code = (type_allocation_code || 'Unknown')
        end
      end

      class DCResponse < Struct.new(:device_id, :capabilities); end

    end
  end
end
