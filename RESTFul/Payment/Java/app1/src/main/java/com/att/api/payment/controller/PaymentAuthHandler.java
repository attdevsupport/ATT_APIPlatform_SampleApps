package com.att.api.payment.controller;

import com.att.api.payment.model.ConfigBean;
import com.att.api.payment.service.PaymentService;
import com.att.api.payment.service.TransactionStatus;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import java.io.BufferedReader;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.StringTokenizer;

public class PaymentAuthHandler {

    private PaymentAuthHandler() {
    }

    public static PaymentAuthHandler getInstance()
    {
      return InstanceHolder.instance;
    }

    private static class InstanceHolder
    {
        static PaymentAuthHandler instance = new PaymentAuthHandler();
    }

    public void postProcessPaymentRequest(HttpServletRequest request, ServletContext servletContext,
            ConfigBean configBean, String transAuthCode, PaymentService paymentService) {
        try {
            TransactionStatus transactionStatus = new TransactionStatus(paymentService.getTransactionStatus());
            saveTransaction(transactionStatus, servletContext,configBean.getTransactionPersistCount());
            request.setAttribute("transactionStatus", transactionStatus);
        } catch (Exception e) {
            request.setAttribute("error",e.getMessage());
        }
    }

    private String getTransactionFilePath(ServletContext servletContext)
    {
        String transactionsPath = "WEB-INF/transactions.txt";
        return servletContext.getRealPath(transactionsPath);
    }

    /**
     * Comma delimitted transaction
     * transaction id,merchant transaction id, consumer id, auth code
     * @param transactionStatus
     * @return
     */
    private String formatTransaction(TransactionStatus transactionStatus) {
        StringBuilder line = new StringBuilder();
        line.append(transactionStatus.getTransactionId());
        line.append(",");
        line.append(transactionStatus.getMerchantTransactionId());
        line.append(",");
        line.append(transactionStatus.getConsumerId());
        line.append(",");
        line.append(transactionStatus.getAuthCode());
        return line.toString();
    }

    private synchronized void saveTransaction(TransactionStatus transactionStatus, ServletContext servletContext, int persistCount) throws IOException {

        BufferedReader fio = null;
        String filePath = getTransactionFilePath(servletContext);
        LinkedList<String> transactions = new LinkedList<String>();
        int tranCount = 0;
        try
        {
            fio = new BufferedReader(new FileReader(filePath));
            String line = null;
            while ((line = fio.readLine()) != null) {
                transactions.add(line);
                tranCount++;
            }
        }
        finally {
            if (fio != null) fio.close();
        }
        //empty transactions
        if (transactions.isEmpty() || tranCount < persistCount)
        {
            FileOutputStream fout = null;
            try
            {
                fout = new FileOutputStream(filePath,true);
                fout.write(formatTransaction(transactionStatus).getBytes());
                fout.write(System.getProperty("line.separator").getBytes());
            }
            finally {
                if (fout != null) fout.close();
            }
        }
        else
        {
            transactions.removeFirst();
            transactions.add(formatTransaction(transactionStatus));
            FileOutputStream fout = null;
            try
            {
                fout = new FileOutputStream(filePath);
                for (int i=0; i < persistCount; i++) {
                    fout.write(transactions.get(i).getBytes());
                    fout.write(System.getProperty("line.separator").getBytes());
                }
            } finally {
                if (fout != null) fout.close();
            }
        }
    }

    public List<TransactionStatus> readTransactions(ServletContext servletContext) throws IOException {
        String filePath = getTransactionFilePath(servletContext);
        BufferedReader bufferedReader = new BufferedReader(new FileReader(filePath));
        String line = null;
        List<TransactionStatus> transList = new ArrayList<TransactionStatus>();
        while ((line = bufferedReader.readLine()) != null) {
            StringTokenizer tokens = new StringTokenizer(line,",");
            TransactionStatus status = new TransactionStatus();
            if (tokens.hasMoreElements()) status.setTransactionId(tokens.nextToken());
            if (tokens.hasMoreElements()) status.setMerchantTransactionId(tokens.nextToken());
            if (tokens.hasMoreElements()) status.setConsumerId(tokens.nextToken());
            if (tokens.hasMoreElements()) status.setAuthCode(tokens.nextToken());
            transList.add(status);
        }
        return transList;
    }

}
