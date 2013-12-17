package com.att.api.immn.service;

public final class MessageListArgs {

    public static class Builder {
        // required
        private final int limit;
        private final int offset;

        // optional
        private String[] msgIds;
        private Boolean isFavorite;
        private Boolean isUnread;
        private MessageType type;
        private String keyword;
        private Boolean isIncoming;

        public Builder(int limit, int offset) {
            this.limit = limit;
            this.offset = offset;

            this.msgIds = null;
            this.isFavorite = null;
            this.isUnread = null;
            this.type = null;
            this.keyword = null;
            this.isIncoming = null;
        }

        public Builder setMessageIds(String[] msgIds)
            { this.msgIds = msgIds; return this; }
        public Builder setIsFavorite(boolean isFavorite)
            { this.isFavorite = isFavorite; return this; }
        public Builder setIsUnread(boolean isUnread)
            { this.isUnread = isUnread; return this; }
        public Builder setType(MessageType type)
            { this.type = type; return this; }
        public Builder setKeyword(String keyword)
            { this.keyword = keyword; return this; }
        public Builder setIsIncoming(boolean isIncoming)
            { this.isIncoming = isIncoming; return this; }
        public MessageListArgs build()
            { return new MessageListArgs(this); }
    }

    private final int limit;
    private final int offset;
    private final String[] msgIds;
    private final Boolean isFavorite;
    private final Boolean isUnread;
    private final MessageType type;
    private final String keyword;
    private final Boolean isIncoming;

    private MessageListArgs(Builder builder) {
        this.limit = builder.limit;
        this.offset = builder.offset;
        this.msgIds = builder.msgIds;
        this.isFavorite = builder.isFavorite;
        this.isUnread = builder.isUnread;
        this.type = builder.type;
        this.keyword = builder.keyword;
        this.isIncoming = builder.isIncoming;
    }

    public int getOffset() {
        return offset;
    }

    public String[] getMessageIds() {
        return msgIds;
    }

    public Boolean isFavorite() {
        return isFavorite;
    }

    public Boolean isUnread() {
        return isUnread;
    }

    public MessageType getType() {
        return type;
    }

    public String getKeyword() {
        return keyword;
    }

    public Boolean isIncoming() {
        return isIncoming;
    }

    public int getLimit() {
        return limit;
    }

}
