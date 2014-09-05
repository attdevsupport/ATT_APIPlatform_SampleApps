require 'tmpdir'
require 'test/unit'
require 'att/codekit/service'

include Att::Codekit::Service

class TestService < Test::Unit::TestCase
  ADDRESSES = ["555-555-5555", "55555555", "", "tel:5555555555", 
               "short:55555555", ""]

  VALID_ADDRESSES = ["tel:5555555555", "short:55555555", "tel:5555555555", 
                     "short:55555555"]

  def test_format_addresses
    addresses = CloudService.format_addresses(ADDRESSES)

    msg = "The parsed address is incorrect! #{addresses}"
    assert_equal(addresses, VALID_ADDRESSES, msg)
  end

  def test_boundary
    boundary = CloudService.generateBoundary
    assert(!boundary.empty?, 
           "Boundary should not be empty!")
  end

  def test_mime_type
    msg = "Incorrect mime type"
    mime = CloudService.getMimeType("/some/path/test.amr")
    assert_equal(mime, "audio/amr", msg)

    mime = CloudService.getMimeType("/some/path/test.mp4")
    assert_equal(mime, "video/mp4", msg)

    mime = CloudService.getMimeType("/some/path/test.ssml")
    assert_equal(mime, "application/ssml+xml", msg)

    mime = CloudService.getMimeType("/some/path/test.wav")
    assert_equal(mime, "audio/x-wav", msg)

    invalid_extension = "lol"
    assert_raise RuntimeError, "#{invalid_extension} did not raise an error" do
      CloudService.getMimeType("/some/path/test.#{invalid_extension}")
    end
  end
end
