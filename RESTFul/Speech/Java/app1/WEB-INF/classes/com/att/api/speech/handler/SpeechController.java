package com.att.api.speech.handler;

import java.io.File;
import java.io.IOException;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.commons.fileupload.FileItem;
import org.apache.commons.fileupload.FileUploadException;
import org.apache.commons.fileupload.disk.DiskFileItemFactory;
import org.apache.commons.fileupload.servlet.ServletFileUpload;

import com.att.api.speech.model.SpeechResponse;
import com.att.api.speech.service.SpeechService;

/**
 * Class used to handle upload request.
 * 
 */
public class SpeechController extends HttpServlet {
	/**
	 * Generated serial version ID.
	 */
	private static final long serialVersionUID = 5612646533168216651L;

	/**
	 * Forwards the specified speech response to the viewer page.
	 * 
	 * @param speechResponse
	 *            speech response to forward.
	 * @param HttpServletRequest
	 *            servlet request
	 * @param HttpServletResponse
	 *            servlet response
	 * @throws ServletException
	 *             if the target resource throws this exception
	 * @throws IOException
	 *             if the target resource throws this exception
	 */
	private void forwardResponse(SpeechResponse speechResponse, HttpServletRequest request,
			HttpServletResponse response) throws ServletException, IOException {
		if (speechResponse != null) {
			request.setAttribute("response", speechResponse);
		}
		RequestDispatcher dispatcher = request.getRequestDispatcher("/Speech.jsp");
		dispatcher.forward(request, response);
	}

	/**
	 * Parses the upload request.
	 * 
	 * @param cfg
	 *            configuration to use for parsing
	 * @param formFieldValues
	 *            form field values to populate
	 * @param HttpServletRequest
	 *            servlet request
	 * @param HttpServletResponse
	 *            servlet response
	 * @throws IOException
	 *             if unable to create a temporary file to store the uploaded
	 *             file
	 * @throws FileUploadException
	 *             if target resource throws this exception
	 * @throws Exception
	 *             if target resource throws this exception
	 */
	private File parseRequest(Config cfg, HashMap<String, String> formFieldValues,
			HttpServletRequest request, HttpServletResponse response) throws IOException,
			FileUploadException, Exception {
		DiskFileItemFactory dfiFactory = new DiskFileItemFactory();
		dfiFactory.setSizeThreshold(cfg.maxUploadFileSize);
		// DiskFileItemFactory uses system temp directory by default

		ServletFileUpload sfUpload = new ServletFileUpload(dfiFactory);

		@SuppressWarnings("unchecked")
		List<FileItem> fItems = sfUpload.parseRequest(request);

		Iterator<FileItem> itr = fItems.iterator();

		File uploadFile = null;

		while (itr.hasNext()) {
			FileItem fItem = itr.next();
			if (fItem.isFormField()) {
				formFieldValues.put(fItem.getFieldName(), fItem.getString());
			} else {
				if (fItem.getName() != "") {
					String filePrefix = "att"; // file prefix used for temp file
					uploadFile = File.createTempFile(filePrefix, ".tmp");
					uploadFile.deleteOnExit();
					fItem.write(uploadFile);
				}
			}
		}

		if (uploadFile == null || !uploadFile.isFile()) {
			String directory = request.getSession().getServletContext().getRealPath("/");
			uploadFile = new File(directory, cfg.defaultFile);
		}

		return uploadFile;
	}

	/**
	 * Handles a GET request.
	 * 
	 * @throws IOException
	 * @throws ServletException
	 * @see HttpServlet
	 */
	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		// Simply redirect to main page
		forwardResponse(null, request, response);
	}

	/**
	 * Handles a POST file upload request.
	 * 
	 * @see HttpServlet
	 */
	@Override
	public void doPost(HttpServletRequest request, HttpServletResponse response)
			throws IOException, ServletException {

		Config cfg = (Config) request.getSession().getAttribute("cfg");
		if (cfg == null) {
			forwardResponse(new SpeechResponse("No configuration supplied"), request, response);
			return;
		}
		if (cfg.getError() != null) {
			forwardResponse(new SpeechResponse(cfg.getError()), request, response);
			return;
		}

		if (!ServletFileUpload.isMultipartContent(request)) {
			forwardResponse(new SpeechResponse("The request is not a multipart request."), request,
					response);
			return;
		}

		try {
			HashMap<String, String> formFieldValues = new HashMap<String, String>();
			File uploadFile = parseRequest(cfg, formFieldValues, request, response);
			String accessToken = (String) request.getSession().getAttribute("accessToken");
			if (accessToken == null || accessToken == "") {
				forwardResponse(new SpeechResponse("Access token has not been set."), request,
						response);
				return;
			}

			String context = formFieldValues.get("context");
			SpeechService speechService = new SpeechService(cfg);

			String chunked = formFieldValues.get("chkChunked");
			if (chunked != null && chunked.equals("Send Chunked")) {
				speechService.setChunked(true);
			}

			SpeechResponse speechResponse = speechService.sendRequest(uploadFile, accessToken,
					context);
			forwardResponse(speechResponse, request, response);
		} catch (Exception e) {
			forwardResponse(new SpeechResponse(e.getMessage()), request, response);
		}
	}
}
