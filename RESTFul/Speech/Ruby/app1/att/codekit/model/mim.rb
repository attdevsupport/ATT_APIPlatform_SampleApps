require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      # Contents of a message
      class MessageContent < ImmutableStruct.new(:attachment, 
                                                 :content_type, 
                                                 :content_length)
        # @!attribute [r] attachment
        #   @return [String] representation of binary data
        # @!attribute [r] content_type
        #   @return [String] type of the attachment
        # @!attribute [r] content_length
        #   @return [String] length of the attachment

        # Check if image
        #
        # @return [Boolean] true if content type is an image
        def image?
          self.content_type.downcase.include? "image"
        end
        # Check if text
        #
        # @return [Boolean] true if content type is text
        def text?
          self.content_type.downcase.include? "text"
        end
        # Check if smil
        #
        # @return [Boolean] true if content type is smil
        def smil?
          self.content_type.downcase.include? "smil"
        end
        # Check if video
        #
        # @return [Boolean] true if content type is video
        def video?
          self.content_type.downcase.include? "video"
        end
        # Check if audio
        #
        # @return [Boolean] true if content type is audio
        def audio?
          self.content_type.downcase.include? "audio"
        end

        # Factory method to create an object from a json string
        #
        # @param response [RestClient::Response] an api response object
        #
        # @return [MessageContent] a parsed object
        def self.createFromResponse(response)
          content_type = response.headers[:content_type]
          content_length = response.headers[:content_length]
          new(response, content_type, content_length)
        end
      end

      # Metadata for a message 
      class MessageContentMetaData < ImmutableStruct.new(:name, :content_type, :type, :url)
        # @!attribute [r] name
        #   @return [String] name of the content
        # @!attribute [r] content_type
        #   @return [String] mime type of the content
        # @!attribute [r] type
        #   @return [String] type of the content
        # @!attribute [r] url
        #   @return [String] url that contains the content

        # Check if image
        #
        # @return [Boolean] true if content type is an image
        def image?
          self.type.downcase.include? "image"
        end
        # Check if text
        #
        # @return [Boolean] true if content type is text
        def text?
          self.type.downcase.include? "text"
        end
        # Check if audio
        #
        # @return [Boolean] true if content type is audio
        def audio?
          self.type.downcase.include? "audio"
        end
        # Check if video
        #
        # @return [Boolean] true if content type is video
        def video?
          self.type.downcase.include? "video"
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [MessageContentMetaData] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [MessageContentMetaData] a parsed object
        def self.createFromParsedJson(json)
          name = json["contentName"]
          content_type = json['contentType'] 
          type = json['type'] 
          url = json['contentUrl']

          new(name, content_type, type, url)
        rescue
          nil
        end
      end

      # A representation of a message object
      class Message < ImmutableStruct.new(:id, :from, :recipients, :subject, :text, 
                                          :timestamp, :favorite, :unread, :type, 
                                          :segmentation_details, :incoming, :content)
        # @!attribute [r] id
        #   @return [String] the message id
        # @!attribute [r] from
        #   @return [String] who sent the message
        # @!attribute [r] recipients
        #   @return [Array<String>] list of who received the message
        # @!attribute [r] subject
        #   @return [String] subject of message (may be nil)
        # @!attribute [r] text
        #   @return [String] message text (may be nil)
        # @!attribute [r] timestamp
        #   @return [String] time of message creation
        # @!attribute [r] favorite
        #   @return [String] deprecated, for internal use only
        # @!attribute [r] unread
        #   @return [Boolean] true if message has not been read
        # @!attribute [r] type
        #   @return [String] type of message
        # @!attribute [r] segmentation_details
        #   @return [SegmentationDetails] preset if message is segmented (may be nil)
        # @!attribute [r] incoming
        #   @return [Boolean] true if message was received
        # @!attribute [r] content
        #   @return [MessageContentMetaData] mms content if present (may be nil)

        # Check if message is a favorite
        #
        # @return [Boolean] true if message is favorited
        def favorite?
          !!self.favorite
        end
        # Check if message is incoming
        #
        # @return [Boolean] true if message is incoming
        def incoming?
          !!self.incoming
        end
        # Check if message is unread
        #
        # @return [Boolean] true if message is unread
        def unread?
          !!self.unread
        end
        # Check if message has been read
        #
        # @return [Boolean] true if message has been read
        def read?
          !self.unread?
        end

        # Check if the message segmented
        # 
        # @return [Boolean] true if message is segmented
        def segmented?
          !!self.segmentation_details
        end
        # Check if the message has subject
        # 
        # @return [Boolean] true if message contains subject
        def subject?
          !!self.subject
        end

        # Check if the message has content
        # 
        # @return [Boolean] true if message contains content
        def content?
          !!self.content
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Message] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Message] a parsed object
        def self.createFromParsedJson(json)
          # Message is either contained in a message object or is the json
          # Depends on the source (single json message or message list)
          msg = (json["message"] || json)

          id = msg["messageId"]
          from = msg["from"]["value"]
          to = parse_recipients(msg["recipients"])
          text = msg["text"]
          timestamp = msg["timestamp"]
          favorite = msg["isFavorite"]
          read = msg["isUnread"]
          type = msg["type"]
          incoming = msg["isIncoming"]
          content = MessageContentMetaData.createFromParsedJson(msg['mmsContent'])

          typeMetaData = msg["typeMetaData"]
          subject = typeMetaData["subject"]
          segmentation_details = SegmentationDetails.createFromParsedJson(typeMetaData["segmentationDetails"])

          new(id, from, to, subject, text, timestamp, favorite, read, type,
              segmentation_details, incoming, content)
        end

        private
        def self.parse_recipients(to)
          list = Array.new
          to.each do |t|
            list << t["value"]
          end
          list
        end
      end

      # Wrapper class to place multiple Message objects into an array
      class Messages
        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Array<Message>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<Message>] a parsed object
        def self.createFromParsedJson(json)
          mlist = Array.new
          Array(json).each do |msg|
            mlist << Message.createFromParsedJson(msg)
          end
          mlist
        end
      end

      # The details of segmentation
      class SegmentationDetails < ImmutableStruct.new(:msg_ref_num, 
                                                      :total_parts, 
                                                      :part_number)
        # @!attribute [r] msg_ref_num
        #   @return [Integer] Unique id, same for all messages of the same segmentation 
        # @!attribute [r] total_parts
        #   @return [Integer] total number of parts for the segmentation
        # @!attribute [r] part_number
        #   @return [Integer] the ordered number that this part exists in the segmentation

        # Check if there are more parts relative to this one
        # 
        # @return [Boolean] true if there are more parts
        def more?
          self.part_number < self.total_parts
        end

        # Check if this is the last part
        # 
        # @return [Boolean] true if this is the last part
        def last?
          self.part_number == self.total_parts
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [SegmentationDetails] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [SegmentationDetails] a parsed object
        def self.createFromParsedJson(json)
          msg_ref_num = json["segmentationMsgRefNumber"]
          total_parts = json["totalNumberOfParts"]
          part = json["thisPartNumber"]

          new(msg_ref_num, total_parts, part)
        rescue
          nil
        end
      end

      # Container for list of messages
      class MessageList < ImmutableStruct.new(:messages, 
                                              :offset, 
                                              :limit,  
                                              :total,
                                              :state,
                                              :cache_status,
                                              :failed_messages)
        # @!attribute [r] messages
        #   @return [Messages] array of message objects
        # @!attribute [r] offset
        #   @return [Integer] the offset that messages were returned from
        # @!attribute [r] limit
        #   @return [Integer] the number of messages requested
        # @!attribute [r] total
        #   @return [Integer] the number of messages actually returned
        # @!attribute [r] state
        #   @return [String] the current state of the mailbox
        # @!attribute [r] cache_status
        #   @return [String] indicates the state of the index cache
        # @!attribute [r] failed_messages
        #   @return [Array<String>] list of message id's that were not able to be retrieved

        # Check if there are failed messages present
        # 
        # @return [Boolean] true if there are failed messages
        def failed_messages?
          !!self.failed_messages
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [MessageList] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [MessageList] a parsed object
        def self.createFromParsedJson(json)
          root = json["messageList"]

          offset = root["offset"].to_i
          limit = root["limit"]
          total = root["total"]
          state = root["state"]
          cache = root["cacheStatus"]
          failed = root["failedMessages"] if root["failedMessages"]
          messages = Messages.createFromParsedJson(root["messages"])

          new(messages, offset, limit, total, state, cache, failed)
        end
      end

      # Index information
      class MessageIndexInfo < ImmutableStruct.new(:status, :state, :message_count) 
        # @!attribute [r] status
        #   @return [String] indicates the state of the index cache
        # @!attribute [r] state
        #   @return [String] A string representation of the state of the mailbox
        # @!attribute [r] message_count
        #   @return [String] number of messages cached 

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [MessageIndexInfo] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [MessageIndexInfo] a parsed object
        def self.createFromParsedJson(json)
          root = json["messageIndexInfo"]

          status = root["status"]
          state = root["state"]
          count = root["messageCount"].to_i

          new(status, state, count)
        end
      end

      # Response of get delta
      class DeltaResponse < ImmutableStruct.new(:state, :deltas)
        # @!attribute [r] state
        #   n@return [String] the state of the mailbox
        # @!attribute [r]  delta
        #   @return [Deltas] object that contains the delta information

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [DeltaResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [DeltaResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json["deltaResponse"]

          state = root["state"]
          deltas = Deltas.createFromParsedJson(root["delta"])

          new(state, deltas)
        end
      end 

      # Deltas representation, wraps multiple Delta objects
      class Deltas 
        # @param json [String] a json encoded string
        #
        # @return [Array<Delta>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<Delta>] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          Array(json).each do |delta|
            list << Delta.createFromParsedJson(delta)
          end
          list
        end
      end

      # Delta representation 
      class Delta < ImmutableStruct.new(:type, :adds, :deletes, :updates)
        # @!attribute [r] type
        #   @return [String] the type of the listed update
        # @!attribute [r] adds
        #   @return [DeltaAdd] object of what has been added
        # @!attribute [r] deletes
        #   @return [DeltaDelete] object of what has been deleted
        # @!attribute [r] updates
        #   @return [DeltaUpdate] object of what has been updated

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Delta] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Delta] a parsed object
        def self.createFromParsedJson(json)
            type = json["type"]
            adds = DeltaAdd.createFromParsedJson(json["adds"])
            deletes = DeltaDelete.createFromParsedJson(json["deletes"])
            updates = DeltaUpdate.createFromParsedJson(json["updates"])

            new(type, adds, deletes, updates)
        end
      end

      # Container of delta information
      class MessageMetadata < Struct.new(:id, :unread, :favorite)
        # @!attribute [rw] id
        #   @return [String] the message id being acted upon
        # @!attribute [rw] read
        #   @return [Boolean] if the message flag read is set
        # @!attribute [rw] favorite
        #   @return [String] deprecated, internal use only

        # Check if message is a favorite
        #
        # @return [Boolean] true if message is favorited
        def favorite?
          !!self.favorite
        end

        # Check if message is unread
        #
        # @return [Boolean] true if message is unread
        def unread?
          !!self.unread
        end

        # Check if message has been read
        #
        # @return [Boolean] true if message has been read
        def read?
          !self.unread?
        end

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [MessageMetadata] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [MessageMetadata] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          Array(json).each do |delta|
            id = delta["messageId"]
            read = delta["isUnread"]
            favorite = delta["isFavorite"]
            list << new(id, read, favorite)
          end
          list
        end
      end

      class DeltaAdd < MessageMetadata
        # @see MessageMetadata
      end
      class DeltaDelete < MessageMetadata
        # @see MessageMetadata
      end
      class DeltaUpdate < MessageMetadata
        # @see MessageMetadata
      end

      # Details about how to access Notifications
      class NotificationDetails < ImmutableStruct.new(:username, 
                                                      :password,
                                                      :https_url,
                                                      :wss_url, 
                                                      :queues)
        # @!attribute [r] username
        #   @return [String] the username for login
        # @!attribute [r] password
        #   @return [String] the password for login
        # @!attribute [r] https_url
        #   @return [String] https url for accessing notifications
        # @!attribute [r] wss_url
        #   @return [String] websocket url for accessing notifications
        # @!attribute [r] queues
        #   @return [Array<String>] location queues can be accessed at

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [NotificationDetails] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [NotificationDetails] a parsed object
        def self.createFromParsedJson(json)
          root = json["notificationConnectionDetails"]

          username = root["username"]
          password = root["password"]
          https = root["httpsUrl"]
          wss = root["wssUrl"]
          queues = root["queues"]

          new(username, password, https, wss, queues)
        end
      end

    end
  end
end
