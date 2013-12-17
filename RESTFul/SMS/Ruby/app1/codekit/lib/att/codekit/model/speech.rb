require 'json'
require 'immutable_struct'

module Att
  module Codekit
    module Model

      # Response of a speech request
      class SpeechResponse < ImmutableStruct.new(:id, :status, :nbest)
        # @!attribute [r] id
        #   @return [String] the id of the request made
        # @!attribute [r] status
        #   @return [String] status of the request made
        # @!attribute [r] nbest
        #   @return [Array<#NBest>] list of nbest objects

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [SpeechResponse] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [SpeechResponse] a parsed object
        def self.createFromParsedJson(json)
          root = json["Recognition"]
          id = root["ResponseId"]
          status = root["Status"]
          nbest = NBest.createFromParsedJson(root["NBest"])

          new(id, status, nbest)
        end
      end

      # Container for a nbest object
      class NBest < ImmutableStruct.new(:confidence, :grade, :hypothesis, 
                                        :language, :nlu_hypothesis, :result, 
                                        :scores, :words)
        # @!attribute [r] confidence
        #   @return [Float] accuracy of request on a scale of 0.0 - 1.0
        # @!attribute [r] grade
        #   @return [String] quantification of the accuracy 
        # @!attribute [r] hypothesis
        #   @return [String] transcription of the audio
        # @!attribute [r] language
        #   @return [String] language used on request 
        # @!attribute [r] nlu_hypothesis
        #   @return [#NLUHypothesis] complex structure containing a set of 
        #     keyword-and-value pairs derived from the speech recognition output (may be nil)
        # @!attribute [r] result
        #   @return [String] similar to hypothesis, usually a formatted version
        # @!attribute [r] scores
        #   @return [Array<Float>] accuracy of each word on a scale of 0.0 - 1.0
        # @!attribute [r] words
        #   @return [Array<String>] list of each word in the response

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Array<NBest>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<NBest>] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          Array(json).each do |nb|
            list << new(nb["Confidence"],
                        nb["Grade"], 
                        nb["Hypothesis"], 
                        nb["LanguageId"],
                        NLUHypothesis.createFromParsedJson(nb["NluHypothesis"]),
                        nb["ResultText"],
                        nb["WordScores"],
                        nb["Words"] )
          end
          list
        end
      end

      #Contains nlu results
      class NLUHypothesis < ImmutableStruct.new(:grammar, :out)
        # @!attribute [r] grammar
        #   @return [String] 
        # @!attribute [r] out
        #   @return [String] 

        # Factory method to create an object from a json string
        #
        # @param json [String] a json encoded string
        #
        # @return [Array<NLUHypothesis>] a parsed object
        def self.createFromJson(json)
          self.createFromParsedJson(JSON.parse(json))
        end

        # Factory method to create an object from a json string
        #
        # @param json [Object] a decoded json string
        #
        # @return [Array<NLUHypothesis>] a parsed object
        def self.createFromParsedJson(json)
          list = Array.new
          if json
            Array(json["OutComposite"]).each do |nlu|
              list << new(nlu["Grammar"], nlu["Out"] )
            end
          end
          list
        end
      end

      #Contains the tts data and content type
      class TTSResponse < ImmutableStruct.new(:data, :type)
        # Response for a text to speech request
        #
        # @!attribute [r] data
        #   @return [String] audio file based on input text
        # @!attribute [r] type
        #   @return [String] content type of the data

        # Factory method to create an object from a restful response
        #
        # @param response [RestClient::Response] a restful response
        #
        # @return [TTSResponse] a parsed object
        def self.createFromResponse(response)
          new(response, response.headers[:content_type])
        end
      end

    end
  end
end
