require '../att_oauth_token'
require 'test/unit'
require 'tmpdir'

include AttCloudServices

class TestOauthToken < Test::Unit::TestCase

  def setup
    @access_token = rand(1_000_000).to_s
    @refresh_token = rand(1_000_000).to_s
    @expiry = Time.now + 60 #add a minute
    @token = OAuthToken.new(@access_token, @expiry, @refresh_token)
    @token_file = File.join(Dir::tmpdir, "token" + Time.now.to_i.to_s + ".tmp")
  end

  def teardown
    File.delete @token_file if File.file? @token_file
  end

  #note ruby 1.8.7 Time doesn't support large numbers so it's should be set
  #to never_expires, thus we should test that our expiry doesn't experience any overflow
  def test_time_overflow
    #test standard case, this should equal NEVER_EXPIRES for ruby <= 1.8.7 after 03:14:07 UTC on Tuesday, 19 January 2038
    assert(@token.expiry.to_i >= OAuthToken::NEVER_EXPIRES, "token set with expiry: #{@expiry}")

    #test hundred years, 
    @token = OAuthToken.new(@access_token, OAuthToken::HUNDRED_YEARS, @refresh_token)
    assert(@token.expiry.to_i >= OAuthToken::NEVER_EXPIRES, "token set with expiry HUNDRED_YEARS: #{OAuthToken::HUNDRED_YEARS}")

    #never expires
    @token = OAuthToken.new(@access_token, OAuthToken::NEVER_EXPIRES, @refresh_token)
    assert(@token.expiry.to_i >= OAuthToken::NEVER_EXPIRES, "token set with expiry NEVER_EXPIRES: #{OAuthToken::NEVER_EXPIRES}")
  end

  #tests that expiration is done correctly
  def test_expiry
    assert(!@token.expired?, "Token shouldn't be expired yet")

    @token = OAuthToken.new(@access_token, Time.now - 1, @refresh_token)
    assert(@token.expired?)
  end

  def test_token_save_load
    OAuthToken.save_token(@token_file, @token)
    file_contents = File.read(@token_file)
    token = OAuthToken.load_token(@token_file)

    msg = "Token differs after saving and loading.\nFile Contents:\n#{file_contents}\n"

    assert_equal(@token, token, msg)
  end

end
