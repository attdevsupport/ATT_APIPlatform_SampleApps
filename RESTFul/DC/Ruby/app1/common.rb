
# Makes sure that valid access_token is stored in the session. Retrieves new tokens if needed.
def obtain_tokens(fqdn, client_id, client_secret, scope, tokens_file)
	if @access_token and @access_token_expires > Time.now
		return
	elsif @refresh_token and @refresh_token_expires > Time.now
		response = RestClient.post "#{fqdn}/oauth/token", :grant_type => 'refresh_token', :client_id => client_id, :client_secret => client_secret, :refresh_token => @refresh_token
	end

	from_json = JSON.parse(response.to_str)
	@access_token = from_json['access_token']
	@refresh_token = from_json['refresh_token']
	#if expires is set to 0 then we store it for 1 year
	if from_json['expires_in'].to_i == 0 then
		@access_token_expires = Time.now + (60*60*24*365*1)
	else
		@access_token_expires = Time.now + (from_json['expires_in'].to_i)/1000
	end
	@refresh_token_expires = Time.now + (60*60*24)
	write_tokens(tokens_file)
end

def write_tokens(tokens_file)
	File.open(tokens_file, 'w+') { |f| f.puts @access_token, @access_token_expires, @refresh_token, @refresh_token_expires }
end
