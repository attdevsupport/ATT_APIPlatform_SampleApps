<?php
namespace Att\Api\Payment;

/*
 * Copyright 2014 AT&T
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

require_once __DIR__ . '/../../lib/Util/FileUtil.php';

use Att\Api\Util\FileUtil;

// TODO: Clean up
class PaymentFileHandler {
    // TODO: Synchronize entirely
    const INDEX_TRANS_ID = 0;
    const INDEX_MERCHANT_TRANS_ID = 1;
    const INDEX_AUTH_CODE = 2;

    const INDEX_SUB_ID = 3;
    const INDEX_S_AUTH_CODE = 4;
    const INDEX_S_MERCHANT_TRANS_ID = 5;
    const INDEX_S_CONSUMER_ID = 6;
    const INDEX_S_MERCHANT_SUB_ID = 7;
    
    private $_fpath;
    private $_limit;

    private function saveFile($fileArr) {
        FileUtil::saveArray($fileArr, $this->_fpath);
    }

    private function loadFile() {
        $fileArr = FileUtil::loadArray($this->_fpath); 
        if ($fileArr == NULL || !is_array($fileArr)) {
            $fileArr = array();
        }
        $indices = array(
                PaymentFileHandler::INDEX_TRANS_ID, 
                PaymentFileHandler::INDEX_MERCHANT_TRANS_ID,
                PaymentFileHandler::INDEX_AUTH_CODE,
                PaymentFileHandler::INDEX_SUB_ID,
                PaymentFileHandler::INDEX_S_AUTH_CODE,
                PaymentFileHandler::INDEX_S_MERCHANT_TRANS_ID,
                PaymentFileHandler::INDEX_S_CONSUMER_ID,
                PaymentFileHandler::INDEX_S_MERCHANT_SUB_ID
                );

        foreach ($indices as $index) {
            if (!isset($fileArr[$index])) {
                $fileArr[$index] = array('Select...');
            }

            // save the Select... string
            $select = array_shift($fileArr[$index]);

            // limit on the number of entires
            while (count($fileArr[$index]) > $this->_limit) { 
                array_shift($fileArr[$index]);
            }

            // now put the Select.. string back
            array_unshift($fileArr[$index], $select);
        }

        return $fileArr;
    }

    public function __construct($fpath, $limit = 5) {
        $this->_fpath = $fpath;
        $this->_limit = $limit;
    }

    public function addTransactionInfo($transId, $merchantTransId, $authCode) {
        $fileArr = $this->loadFile();

        // Only add if unique
        if (in_array($transId, $fileArr[PaymentFileHandler::INDEX_TRANS_ID])) {
            return;
        }

        array_push($fileArr[PaymentFileHandler::INDEX_TRANS_ID], $transId);
        array_push($fileArr[PaymentFileHandler::INDEX_MERCHANT_TRANS_ID], 
                $merchantTransId);
        array_push($fileArr[PaymentFileHandler::INDEX_AUTH_CODE], $authCode);
        $this->saveFile($fileArr);
    }

    public function addSubscriptionInfo($subId, $sMerchantId, $authCode, 
            $consumerId, $merchantSubId) {

        $fileArr = $this->loadFile();

        // Only add if unique
        if (in_array($subId, $fileArr[PaymentFileHandler::INDEX_SUB_ID])) {
            return;
        }

        array_push($fileArr[PaymentFileHandler::INDEX_SUB_ID], $subId);
        array_push($fileArr[PaymentFileHandler::INDEX_S_MERCHANT_TRANS_ID], 
                $sMerchantId);
        array_push($fileArr[PaymentFileHandler::INDEX_S_AUTH_CODE], 
                $authCode);
        array_push($fileArr[PaymentFileHandler::INDEX_S_CONSUMER_ID], 
                $consumerId);
        array_push($fileArr[PaymentFileHandler::INDEX_S_MERCHANT_SUB_ID], 
                $merchantSubId);
        $this->saveFile($fileArr);
    }

    public function getFileValues() {
        return $this->loadFile();
    }
}

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */
?>
