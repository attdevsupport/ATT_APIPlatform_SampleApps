
module Att
  module Codekit
    module Model

      #container for a speech result
      class SpeechResponse < Struct.new(:id, :status, :nbest); end

      #Container for a nbest object
      class NBest < Struct.new(:confidence, :grade, :hypothesis, :language, :nlu_hypothesis, :result, :scores, :words); end

      #Contains nlu results
      class NLUHypothesis < Struct.new(:grammar, :out); end

      #Contains the tts data and content type
      class TTSResponse < Struct.new(:data, :type); end

    end
  end
end
