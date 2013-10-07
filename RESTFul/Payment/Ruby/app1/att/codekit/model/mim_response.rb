
module Att
  module Codekit
    module Model

      class MessageContent < Struct.new(:attachment, :content_type)
        def image?
          self.content_type.downcase.include? "image"
        end
        def text?
          self.content_type.downcase.include? "text"
        end
        def smil?
          self.content_type.downcase.include? "smil"
        end
      end

      class MMSHeader < Struct.new(:id, :from, :to, :subject, :received, :text, :favorite, :read, :type, :direction, :content);
        def favorite?
          self.favorite
        end
        def read?
          self.read
        end
      end

      class ContentID < Struct.new(:type, :name, :part); end

      class MessageHeaders  
        attr_reader :headers, :headercount, :cursor

        # Create a MessageHeader object
        #
        # @param headers [Array<MMSHeader>] an array of headers
        # @param headercount [Integer] the number of headers
        # @param cursor [String] the index cursor that specifies the next block of messages
        def initialize(headers, headercount, cursor)
          @headers = Array(headers)
          @headercount = headercount
          @cursor = cursor
        end

        #iterate through the message headers
        def each
          @headers.each {|x| yield x}
        end
      end

    end
  end
end
