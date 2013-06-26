require 'att_api_service'

module AttCloudServices
  module Speech
    SCOPE = 'SPEECH'
    INVALID_FILETYPE = "Invalid file type, use audio/wav, audio/x-wav, audio/amr, audio/amr-wb or x-speex."

    module ENDPOINTS
     SPEECH = "/speech/v3/speechToText"
    end

    class SpeechService < AttApiService

      def speechToText(xArgs, file, context, chunked, filetype=nil)
        xArgs ||= "" #set to empty string if nil
        x_arg_val = URI.escape(xArgs)

        filecontents = File.read(file)

        filetype ||= getFiletype file

        headers = {
          :X_arg => "#{x_arg_val}",
          :X_SpeechContext => context,
          :Content_Type => "#{filetype}"
        }

        if chunked then
          headers.merge!(:Content_Transfer_Encoding => 'chunked')
        end

        url = "#{@oauth.fqdn}#{ENDPOINTS::SPEECH}"

        self.postRequest(url, filecontents, headers)
      end

      #Basic file extension check to ensure proper file types are uploaded
      #Some times browser's may recognize mime types are
      #application/octet-stream if the system does not know about the files
      #mime type
      def getFiletype filename
        extension = filename.split(".")[1]
        case extension
                when "wav" then	"audio/wav"
                when "amr" then	"audio/amr"
                when "amr-wb" then "audio/amr-wb"
                when "x-speex" then "audio/x-speex"
                when "spx" then	"audio/x-speex"
                else 
                end
      end

    end
  end
end
