<?php
namespace Att\Api\Speech;

/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4: */

/**
 * Speech API Library
 * 
 * PHP version 5.4+
 * 
 * LICENSE: Licensed by AT&T under the 'Software Development Kit Tools 
 * Agreement.' 2014. 
 * TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTIONS:
 * http://developer.att.com/sdk_agreement/
 *
 * Copyright 2014 AT&T Intellectual Property. All rights reserved.
 * For more information contact developer.support@att.com
 * 
 * @category  API 
 * @package   Speech 
 * @author    pk9069
 * @copyright 2014 AT&T Intellectual Property
 * @license   http://developer.att.com/sdk_agreement AT&amp;T License
 * @link      https://developer.att.com/docs/apis/rest/3/Speech
 */

/**
 * Immutable class that represents the N-Best hypothesis for speech recognization.
 *
 * @category API
 * @package  Speech
 * @author   pk9069
 * @license  http://developer.att.com/sdk_agreement AT&amp;T License
 * @version  Release: @package_version@ 
 * @link     https://developer.att.com/docs/apis/rest/3/Speech
 */
final class NBest
{
    /**
     * hypothesis.
     *
     * @var string
     */
    private $_hypothesis;

    /**
     * Language id used to decode speech.
     *
     * @var string
     */
    private $_languageId;

    /**
     * Confidence level value, where value: 0 <= value <= 1.0
     *
     * @var string
     */
    private $_confidence;

    /**
     * Assessment of quality.
     *
     * @var string
     */
    private $_grade;

    /**
     * Result text.
     *
     * @var string
     */
    private $_resultText;
    
    /**
     * Words recognized from speech.
     *
     * @var array 
     */
    private $_words;

    /**
     * Confidence score for each word.
     *
     * @var array
     */
    private $_wordScores;

    /**
     * Creates an N-Best hypothesis object.
     *
     * @param string $hypothesis hypothesis for speech recognization
     * @param string $languageId language Id
     * @param string $confidence confidence
     * @param string $grade      grade
     * @param string $resultText result text
     * @param array  $words      array of words
     * @param array  $wordScores array of word scores
     */
    public function __construct($hypothesis, $languageId, $confidence, $grade,
        $resultText, $words, $wordScores
    ) {
        $this->_hypothesis = $hypothesis;
        $this->_languageId = $languageId;
        $this->_confidence = $confidence;
        $this->_grade = $grade;
        $this->_resultText = $resultText;
        $this->_words = $words;
        $this->_wordScores = $wordScores;
    }

    /**
     * Gets the hypothesis.
     *
     * @return string hypothesis
     */
    public function getHypothesis()
    {
        return $this->_hypothesis;
    }

    /**
     * Gets the language ID.
     *
     * @return language Id
     */
    public function getLanguageId()
    {
        return $this->_languageId;
    }

    /**
     * Gets the confidence for word recognization.
     *
     * @return string confidence
     */
    public function getConfidence()
    {
        return $this->_confidence;
    }

    /**
     * Gets grade.
     *
     * @return string Grade
     */
    public function getGrade()
    {
        return $this->_grade;
    }

    /**
     * Gets result text.
     *
     * @return string result text
     */
    public function getResultText()
    {
        return $this->_resultText;
    }

    /**
     * Gets an array of words.
     *
     * @return array words
     */
    public function getWords()
    {
        return $this->_words;
    }

    /**
     * Gets an array of word scores.
     *
     * @return array word scores
     */
    public function getWordScores()
    {
        return $this->_wordScores;
    }

    public static function fromArray($arr)
    {
        $nBest = new NBest(
            $arr['Hypothesis'],
            $arr['LanguageId'],
            $arr['Confidence'],
            $arr['Grade'],
            $arr['ResultText'],
            $arr['Words'],
            $arr['WordScores']
        );

        return $nBest;
    }
}

?>
