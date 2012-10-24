<!-- 
Licensed by AT&T under 'Software Development Kit Tools Agreement.' 2012
TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION: http://developer.att.com/sdk_agreement/
Copyright 2012 AT&T Intellectual Property. All rights reserved. http://developer.att.com
For more information contact developer.support@att.com
-->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xml:lang="en" xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title>AT&amp;T Sample Advertisement Application - Get
	Advertisement Application</title>
<meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
<link rel="stylesheet" type="text/css" href="style/common.css" />
</head>
<body>

	<%@ page contentType="text/html; charset=iso-8859-1" language="java"%>
	<%@ page import="com.att.api.ads.handler.AdHandler"%>
	<%@ page import="com.att.api.ads.model.AdsResponse"%>
	<%@ page import="com.att.api.util.DateUtil"%>
	<%@ include file="getToken.jsp"%>
	<%
	AdHandler adHandler = new AdHandler(endPoint,UDID, adType, request);
	AdsResponse model = adHandler.processRequest();
%>
	<div id="container">
		<form name="input" method="post">
			<!-- open HEADER -->
			<div id="header">
				<div>
					<div class="hcRight"><%=DateUtil.getServerTime()%></div>
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
			<div>
				<div class="content">
					<h1>AT&amp;T Sample Advertisement Application - Get
						Advertisement Application</h1>
					<h2>Feature 1: Get Advertisement</h2>
				</div>
			</div>
			<br /> <br />
			<div class="extra">
				<table border="0" width="100%">
					<tbody>
						<tr>
							<td width="10%" class="label"
								title="Zip/Postal code of a user. For US only. (Integer)">
								Zip Code:</td>
							<td class="cell"><input type="text" id="zipCode"
								name="zipCode"
								value="<%=model.getAttribute(request,"zipCode")%>" /></td>
						</tr>
						<tr>
						</tr>
						<tr>
							<td width="10%" class="label"
								title="User location latitude value (in degrees).">
								Country:</td>
							<td class="cell"><input type="text" name="country"
								id="country" value="<%=model.getAttribute(request,"country")%>" />
							</td>
						</tr>
						<tr>
						</tr>
						<tr>
						</tr>
						<tr>
							<td width="10%" class="label">Over 18 Ad Content:</td>
							<td class="cell"><select name="over18" id="over18">
									<option value=""></option>
									<option value="0" <%=model.isSelected(request,"over18","0")%>>Deny
										Over 1</option>
									<option value="2" <%=model.isSelected(request,"over18","2")%>>Only
										Over 18</option>
									<option value="3" <%=model.isSelected(request,"over18","3")%>>Allow
										All Ads</option>
							</select></td>
						</tr>
					</tbody>
				</table>
				<br />
				<table>
					<tbody>
						<div id="extraleft">
							<div class="warning">
								<strong>Note:</strong><br /> All Parameters are optional except
								Category.<br /> If this application is accessed from desktop
								browser, you may see successful response without Ads (HTTP 204).
							</div>
						</div>
					</tbody>
				</table>
			</div>
			<div class="navigation">
				<table border="0" width="100%">
					<tbody>
						<tr>
							<td width="10%" class="label">*Category:</td>
							<td class="cell"><select name="category" id="category">
									<option value="auto"
										<%=model.isSelected(request,"category","auto")%>>auto</option>
									<option value="business"
										<%=model.isSelected(request,"category","business")%>>business</option>
									<option value="chat"
										<%=model.isSelected(request,"category","chat")%>>chat</option>
									<option value="communication"
										<%=model.isSelected(request,"category","communication")%>>communication</option>
									<option value="community"
										<%=model.isSelected(request,"category","community")%>>community</option>
									<option value="entertainment"
										<%=model.isSelected(request,"category","entertainment")%>>entertainment</option>
									<option value="finance"
										<%=model.isSelected(request,"category","finance")%>>finance</option>
									<option value="games"
										<%=model.isSelected(request,"category","games")%>>games</option>
									<option value="health"
										<%=model.isSelected(request,"category","health")%>>health</option>
									<option value="local"
										<%=model.isSelected(request,"category","local")%>>local</option>
									<option value="maps"
										<%=model.isSelected(request,"category","maps")%>>maps</option>
									<option value="medical"
										<%=model.isSelected(request,"category","medical")%>>medical</option>
									<option value="movies"
										<%=model.isSelected(request,"category","movies")%>>movies</option>
									<option value="music"
										<%=model.isSelected(request,"category","music")%>>music</option>
									<option value="news"
										<%=model.isSelected(request,"category","news")%>>news</option>
									<option value="other"
										<%=model.isSelected(request,"category","other")%>>other</option>
									<option value="personals"
										<%=model.isSelected(request,"category","personals")%>>personals</option>
									<option value="photos"
										<%=model.isSelected(request,"category","photos")%>>photos</option>
									<option value="shopping"
										<%=model.isSelected(request,"category","shopping")%>>shopping</option>
									<option value="social"
										<%=model.isSelected(request,"category","social")%>>social</option>
									<option value="sports"
										<%=model.isSelected(request,"category","sports")%>>sports</option>
									<option value="technology"
										<%=model.isSelected(request,"category","technology")%>>technology</option>
									<option value="tools"
										<%=model.isSelected(request,"category","tools")%>>tools</option>
									<option value="travel"
										<%=model.isSelected(request,"category","travel")%>>travel</option>
									<option value="tv"
										<%=model.isSelected(request,"category","tv")%>>tv</option>
									<option value="video"
										<%=model.isSelected(request,"category","video")%>>video</option>
									<option value="weather"
										<%=model.isSelected(request,"category","weather")%>>weather</option>
							</select></td>
							<td width="10%" class="label" title="MMA Size in pixels">
								MMA Size:</td>
							<td class="cell"><select name="MMA" id="MMA">
									<option value=""></option>
									<option value="120 x 20"
										<%=model.isSelected(request, "MMA", "120 x 20") %>>120
										x 20</option>
									<option value="168 x 28"
										<%=model.isSelected(request, "MMA", "168 x 28") %>>168
										x 28</option>
									<option value="216 x 36"
										<%=model.isSelected(request, "MMA", "216 x 36") %>>216
										x36</option>
									<option value="300 x 50"
										<%=model.isSelected(request, "MMA", "300 x 50") %>>300
										x 50</option>
									<option value="300 x 250"
										<%=model.isSelected(request, "MMA","300 x 250") %>>300
										x 250</option>
									<option value="320 x 50"
										<%=model.isSelected(request, "MMA", "320 x 50") %>>320
										x 50</option>
							</select></td>
						</tr>
						<tr>
							<td width="10%" class="label"
								title="The City of the user. For US only.">City:</td>
							<td class="cell"><input type="text" name="city" id="city"
								value="<%=model.getAttribute(request, "city") %>" /></td>
							<td width="10%" class="label"
								title="Area code of a user. For US only. (Integer)">Area
								Code:</td>
							<td class="cell"><input type="text" name="areaCode"
								id="areaCode"
								value="<%=model.getAttribute(request, "areaCode") %>" /></td>
						</tr>
						<tr>
							<td width="10%" class="label"
								title="Country of user. An ISO 3166 code to be used for specifying a country code.">
								Latitude:</td>
							<td class="cell"><input type="text" name="Latitude"
								id="Latitiude"
								value="<%=model.getAttribute(request, "Latitude") %>" /></td>
							<td width="10%" class="label"
								title="User location longitude value (in degrees).">
								Longitude:</td>
							<td class="cell"><input type="text" name="Longitude"
								id="Longitude"
								value="<%=model.getAttribute(request, "Longitude") %>" /></td>
						</tr>
						<tr>
						</tr>
						<tr>
							<td width="10%" class="label">Age Group:</td>
							<td class="cell"><select name="ageGroup" id="ageGroup">
									<option value=""></option>
									<option value="1-13"
										<%=model.isSelected(request,"ageGroup","1-13")%>>1-13</option>
									<option value="14-25"
										<%=model.isSelected(request,"ageGroup","14-25")%>>14-25</option>
									<option value="26-35"
										<%=model.isSelected(request,"ageGroup","26-35")%>>26-35</option>
									<option value="36-55"
										<%=model.isSelected(request,"ageGroup","36-55")%>>36-55</option>
									<option value="55-100"
										<%=model.isSelected(request,"ageGroup","55-100")%>>55-100</option>
							</select></td>
							<td width="10%" class="label">Premium:</td>
							<td class="cell"><select name="Premium" id="Premium">
									<option value=""></option>
									<option value="0" <%=model.isSelected(request,"premium","0")%>>Non
										Premium</option>
									<option value="1" <%=model.isSelected(request,"premium","1")%>>Premium
										Only</option>
									<option value="2" <%=model.isSelected(request,"premium","2")%>>Both</option>
							</select></td>
						</tr>
						<tr>
							<td width="10%" class="label">Gender:</td>
							<td class="cell"><select name="gender" id="gender">
									<option value=""></option>
									<option value="M" <%=model.isSelected(request,"gender","M")%>>Male</option>
									<option value="F" <%=model.isSelected(request,"gender","F")%>>Female</option>
							</select></td>
							<td width="10%" class="label"
								title="Filter ads by keywords (delimited by commas Ex: music,singer)">
								Keywords:</td>
							<td class="cell"><input type="text" name="keywords"
								id="keywords"
								value="<%=model.getAttribute(request,"keywords")%>" /></td>
						</tr>
						<tr>
						</tr>
					</tbody>
				</table>
				<br />
				<table border="0" width="100%">
					<tbody>
						<tr valign="middle" align="right">
							<td class="cell" width="35%">
								<button type="submit" name="btnGetAds">Get
									Advertisement</button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</form>
		<br style="clear: both;" />
		<div align="center">
			<div id="statusPanel"
				style="font-family: Calibri; font-size: XX-Small;">
				<% 
				if (model.getStatusCode() == 204)
				{ %>
				<div class="successWide" align="left"
					style="font-family: Sans-serif; font-size: 9pt;">
					<strong>Success:</strong><br />No Ads returned.
				</div>
				<%	
				}
				else
				if (model.getStatusCode() == 200)
				{
				%>
				<table class="successWide" align="left"
					style="font-family: Sans-serif; font-size: 9pt;">
					<tbody>
						<tr>
							<td style="font-weight: bold; width: 20%;">Success:</td>
						</tr>
						<%
							if (model.getAdType().equals("image"))
							{
							%>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">Type:
							</td>
							<td class="cell">image</td>
						</tr>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">ClickUrl:</td>
							<td class="cell"><%=model.getClickUrl()%></td>
						</tr>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">ImageUrl.Image:
							</td>
							<td class="cell"><%=model.getImageUrl()%></td>
						</tr>
						<%
							}
							else if (model.getAdType().equals("text"))
							{
							%>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">Type:
							</td>
							<td class="cell"><%=model.getAdType()%></td>
						</tr>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">ClickUrl:</td>
							<td class="cell"><%=model.getClickUrl()%></td>
						</tr>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">Text:
							</td>
							<td class="cell"><%=model.getAdText()%></td>
						</tr>
						<%
							}	
							else if (model.getAdType().equals("thirdparty"))
							{
							%>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">Type:
							</td>
							<td class="cell"><%=model.getAdType()%></td>
						</tr>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">ClickUrl:</td>
							<td class="cell"><%=model.getClickUrl()%></td>
						</tr>
						<tr>
							<td class="label" align="right" style="font-weight: bold;">Text:
							</td>
							<td class="cell"><%=model.getAdText()%></td>
						</tr>
						<%
							}
							%>
					</tbody>
				</table>
				<%
					if (model.getAdType().equals("image"))
					{
					%>
				<br></br> <img src="<%=model.getImageUrl()%>"></img>
				<%
					}
					else if (model.getAdContent() != null)
					{
					%>
				<br></br>
				<%=model.getAdContent() %>
				<%
					}
					%>
				<%
			}
			else if (model.isFormStatus())
			{
			%>
				<% 
		                //Input validation error
		                if (model.getErrors().size() > 0) 
		                {
		                %>
							<table class="errorWide"
								style="font-family: sans-serif; font-size: 9pt;">
								<tr>
									<td style="font-weight: bold;">ERROR:</td>
								</tr>
								<tr>
									<td>Please correct the following error(s):
										<ul>
											<% for (String error: model.getErrors())
									                	{
									                	%>
											<li><%=error%></li>
											<%
														}
									                	%>
										</ul>
									</td>
								</tr>
							</table>
				<%
		                }
		                //Service errors
						else 
						{
		                %>
						<div class="errorWide">
							<strong>ERROR:</strong><br /> <strong>Status:</strong><%=model.getStatusCode()%><br />
							<%=model.getErrorResponse()%>
						</div>
				<%
		                } 
		                %>
				<%
		      } %>
				<br />
			</div>
		</div>
		<div id="footer" align="center">
			<div
				style="float: right; width: 20%; font-size: 9px; text-align: right">
				Powered by AT&amp;T Cloud Architecture</div>
			<p>
				© 2012 AT&amp;T Intellectual Property. All rights reserved. <a
					href="http://developer.att.com/" target="_blank">http://developer.att.com</a>
				<br /> The Application hosted on this site are working examples
				intended to be used for reference in creating products to consume
				AT&amp;T Services and not meant to be used as part of your product.
				The data in these pages is for test purposes only and intended only
				for use as a reference in how the services perform. <br /> For
				download of tools and documentation, please go to <a
					href="https://devconnect-api.att.com/" target="_blank">https://devconnect-api.att.com</a>
				<br /> For more information contact <a
					href="mailto:developer.support@att.com">developer.support@att.com</a>
			</p>
		</div>
	</div>
</body>
</html>
