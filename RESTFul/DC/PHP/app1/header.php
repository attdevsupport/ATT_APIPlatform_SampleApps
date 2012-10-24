<?php
/*
Licensed by AT&T under 'Software Development Kit Tools Agreement.' September 2011
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2011 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
*/
?>
        <!-- open HEADER -->
        <div id="header">
        	<div>
		        <div class="hcRight">
    			<?php echo  date("D M j G:i:s T Y"); ?>

	        	</div>
    		    <div class="hcLeft">Server Time:</div>
	        </div>
    	    <div>
	    	    <div class="hcRight">
		    	    <script language="JavaScript" type="text/javascript">
                        var myDate = new Date();
                        document.write(myDate);
                    </script>
                </div>
		        <div class="hcLeft">Client Time:</div>
    	    </div>
	        <div>
		        <div class="hcRight">
			        <script language="JavaScript" type="text/javascript">
                        document.write("" + navigator.userAgent);
                    </script>
    		    </div>
	    	    <div class="hcLeft">User Agent:</div>
            </div>
	        <br clear="all" />
        </div>
        <!-- close HEADER -->
