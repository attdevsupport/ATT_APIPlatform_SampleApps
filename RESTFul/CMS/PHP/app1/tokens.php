<?php

    function RefreshToken($FQDN, $api_key, $secret_key, $scope, $fullToken)
    {

        $refreshToken = $fullToken["refreshToken"];
        $accessTok_Url = $FQDN . "/oauth/token";

        //http header values
        $accessTok_headers = array(
            'Content-Type: application/x-www-form-urlencoded'
        );

        //Invoke the URL
        $post_data = "client_id=" . $api_key . "&client_secret=" . $secret_key . "&refresh_token=" . $refreshToken . "&grant_type=refresh_token";

        $accessTok = curl_init();
        curl_setopt($accessTok, CURLOPT_URL, $accessTok_Url);
        curl_setopt($accessTok, CURLOPT_HTTPGET, 1);
        curl_setopt($accessTok, CURLOPT_HEADER, 0);
        curl_setopt($accessTok, CURLINFO_HEADER_OUT, 0);
        curl_setopt($accessTok, CURLOPT_HTTPHEADER, $accessTok_headers);
        curl_setopt($accessTok, CURLOPT_RETURNTRANSFER, 1);
        curl_setopt($accessTok, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($accessTok, CURLOPT_POST, 1);
        curl_setopt($accessTok, CURLOPT_POSTFIELDS, $post_data);
        $accessTok_response = curl_exec($accessTok);
        $currentTime = time();

        $responseCode = curl_getinfo($accessTok, CURLINFO_HTTP_CODE);
        if ($responseCode == 200) {
            $jsonObj = json_decode($accessTok_response);
            $accessToken = $jsonObj->{'access_token'}; //fetch the access token from the response.
            $refreshToken = $jsonObj->{'refresh_token'};
            $expiresIn = $jsonObj->{'expires_in'};

            if ($expiresIn == 0) {
                $expiresIn = 24 * 60 * 60;

            }

            $refreshTime = $currentTime + (int)($expiresIn); // Time for token refresh
            $updateTime = $currentTime + (24 * 60 * 60); // Time to get for a new token update, current time + 24h

            $fullToken["accessToken"] = $accessToken;
            $fullToken["refreshToken"] = $refreshToken;
            $fullToken["refreshTime"] = $refreshTime;
            $fullToken["updateTime"] = $updateTime;

        } else {
            $fullToken["accessToken"] = null;
            $fullToken["errorMessage"] = curl_error($accessTok) . $accessTok_response;


        }
        curl_close($accessTok);
        return $fullToken;

    }

    function GetAccessToken($FQDN, $api_key, $secret_key, $scope)
    {

        $accessTok_Url = $FQDN . "/oauth/token";

        //http header values
        $accessTok_headers = array(
            'Content-Type: application/x-www-form-urlencoded'
        );

        //Invoke the URL
        $post_data = "client_id=" . $api_key . "&client_secret=" . $secret_key . "&scope=" . $scope . "&grant_type=client_credentials";

        $accessTok = curl_init();
        curl_setopt($accessTok, CURLOPT_URL, $accessTok_Url);
        curl_setopt($accessTok, CURLOPT_HTTPGET, 1);
        curl_setopt($accessTok, CURLOPT_HEADER, 0);
        curl_setopt($accessTok, CURLINFO_HEADER_OUT, 0);
        curl_setopt($accessTok, CURLOPT_HTTPHEADER, $accessTok_headers);
        curl_setopt($accessTok, CURLOPT_RETURNTRANSFER, 1);
        curl_setopt($accessTok, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($accessTok, CURLOPT_POST, 1);
        curl_setopt($accessTok, CURLOPT_POSTFIELDS, $post_data);
        $accessTok_response = curl_exec($accessTok);


        $responseCode = curl_getinfo($accessTok, CURLINFO_HTTP_CODE);
        $currentTime = time();
        /*
         If URL invocation is successful fetch the access token and store it in session,
         else display the error.
        */
        if ($responseCode == 200) {
            $jsonObj = json_decode($accessTok_response);
            $accessToken = $jsonObj->{'access_token'}; //fetch the access token from the response.
            $refreshToken = $jsonObj->{'refresh_token'};
            $expiresIn = $jsonObj->{'expires_in'};

            if ($expiresIn == 0) {
                $expiresIn = 24 * 60 * 60 * 365 * 100;

            }
            $refreshTime = $currentTime + (int)($expiresIn); // Time for token refresh
            $updateTime = $currentTime + (24 * 60 * 60); // Time to get a new token update, current time + 24h

            $fullToken["accessToken"] = $accessToken;
            $fullToken["refreshToken"] = $refreshToken;
            $fullToken["refreshTime"] = $refreshTime;
            $fullToken["updateTime"] = $updateTime;

        } else {

            $fullToken["accessToken"] = null;
            $fullToken["errorMessage"] = curl_error($accessTok) . $accessTok_response;

        }
        curl_close($accessTok);
        return $fullToken;
    }

    function SaveToken($fullToken, $oauth_file)
    {

        $accessToken = $fullToken["accessToken"];
        $refreshToken = $fullToken["refreshToken"];
        $refreshTime = $fullToken["refreshTime"];
        $updateTime = $fullToken["updateTime"];


        $tokenfile = $oauth_file;
        $fh = fopen($tokenfile, 'w');
        $tokenfile = "<?php \$accessToken=\"" . $accessToken . "\"; \$refreshToken=\"" . $refreshToken . "\"; \$refreshTime=" . $refreshTime . "; \$updateTime=" . $updateTime . "; ?>";
        fwrite($fh, $tokenfile);
        fclose($fh);
    }

    function check_token($FQDN, $api_key, $secret_key, $scope, $fullToken, $oauth_file)
    {

        $currentTime = time();

        if (($fullToken["updateTime"] == null) || ($fullToken["updateTime"] <= $currentTime)) {
            $fullToken = GetAccessToken($FQDN, $api_key, $secret_key, $scope);
            if ($fullToken["accessToken"] == null) {
                      //echo $fullToken["errorMessage"];
            } else {
                      //echo $fullToken["accessToken"];
                SaveToken($fullToken, $oauth_file);
            }
        } elseif ($fullToken["refreshTime"] <= $currentTime) {
            $fullToken = RefreshToken($FQDN, $api_key, $secret_key, $scope, $fullToken);
            if ($fullToken["accessToken"] == null) {
                      //echo $fullToken["errorMessage"];
            } else {
                      //echo $fullToken["accessToken"];
                SaveToken($fullToken, $oauth_file);
            }
        }

        return $fullToken;

    }

?>