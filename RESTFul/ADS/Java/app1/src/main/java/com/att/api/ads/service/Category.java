/* vim: set expandtab tabstop=4 shiftwidth=4 softtabstop=4 */

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

package com.att.api.ads.service;

public enum Category {
    AUTO("auto"), BUSINESS("business"), FINANCE("finance"), CHAT("chat"),
    COMMUNITY("community"), SOCIAL("social"), PERSONALS("personals"),
    COMMUNICATION("communication"), TECHNOLOGY("technology"), GAMES("games"),
    HEALTH("health"), MEDICAL("medical"), MAPS("maps"), LOCAL("local"),
    ENTERTAINMENT("entertainment"), MOVIES("movies"), TV("tv"), MUSIC("music"),
    PHOTOS("photos"), VIDEO("video"), NEWS("news"), WEATHER("weather"),
    SPORTS("sports"), SHOPPING("shopping"), TOOLS("tools"), TRAVEL("travel"),
    OTHER("other");

    private final String val;

    private Category(String val) {
        this.val = val;
    }

    public String getString() {
        return this.val;
    }

    public static Category fromString(final String str) {
        Category[] categories = Category.values();
        for (Category c : categories) {
            if (c.getString().equals(str)) {
                return c;
            }
        }

        return null;
    }

    public static String[] stringValues() {
        Category[] categories = Category.values();
        String[] strs = new String[categories.length];

        for (int i = 0; i < categories.length; ++i) {
            strs[i] = categories[i].getString();
        }

        return strs;
    }
}
