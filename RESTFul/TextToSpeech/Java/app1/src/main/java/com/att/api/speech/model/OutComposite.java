package com.att.api.speech.model;

public class OutComposite {
    private String grammar;
    private String out;

    /**
     *
     */
    public OutComposite(String grammar, String out) {
        this.grammar = grammar;
        this.out = out;
    }

    /**
     * @return the grammar
     */
    public String getGrammar() {
        return grammar;
    }

    /**
     * @return the out
     */
    public String getOut() {
        return out;
    }
}
