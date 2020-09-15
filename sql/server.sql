
CREATE TABLE user(
    user_id int auto_increment not null,
    username varchar(13) not null,
    pwd char(44) not null,
    salt char(44) not null,
    creation_date timestamp DEFAULT CURRENT_TIMESTAMP,
    last_logged_in timestamp DEFAULT CURRENT_TIMESTAMP,
    is_logged_in int(1) DEFAULT '0',
    is_deleted int(1) DEFAULT '0',
    permission int DEFAULT '0',
    PRIMARY KEY(user_id)
);

-- Character is using hard-delete script since on delete cascade
CREATE TABLE characterx(
    character_id int auto_increment,
    user_id int,
    char_name varchar(20) not null,
    hat_id int DEFAULT '0',
    top_id int DEFAULT '0',
    bottom_id int DEFAULT '0',
    xpos float DEFAULT '0',
    ypos float DEFAULT '0',
    zpos float DEFAULT '0',
    PRIMARY KEY(character_id),
    CONSTRAINT fk_user_character
    FOREIGN KEY (user_id) 
    REFERENCES user(user_id)
        ON DELETE CASCADE
);

CREATE TABLE inventory(
    inv_id int auto_increment,
    character_id int not null,
    item int not null,
    itemIdx int,
    is_equipped int(1) DEFAULT '0',
    PRIMARY KEY(inv_id),
    CONSTRAINT fk_inv_character
    FOREIGN KEY (character_id) REFERENCES characterx(character_id) ON DELETE CASCADE
);

-- done
CREATE TABLE quiz(
    quiz_id int auto_increment,
    quiz_name text,
    PRIMARY KEY(quiz_id)
);
-- done
CREATE TABLE topic(
    topic_id int auto_increment,
    topic text not null,
    topic_desc text not null,
    PRIMARY KEY(topic_id)
);

-- done
CREATE TABLE question(
    question_id int auto_increment,
    question text,
    difficulty int DEFAULT '1',
    topic_id int,
    CONSTRAINT fk_question_topic
    FOREIGN KEY(topic_id) REFERENCES topic(topic_id) ON DELETE CASCADE ON UPDATE CASCADE,
    PRIMARY KEY(question_id)
);

-- done
CREATE TABLE answer(
    answer_id int,
    question_id int,
    answer_text varchar(200) not null,
    is_correct char(1) DEFAULT 'N',
    CONSTRAINT fk_answer_question
    FOREIGN KEY(question_id) REFERENCES question(question_id) ON DELETE CASCADE ON UPDATE CASCADE,
    PRIMARY KEY(question_id, answer_id)
);

CREATE TABLE session(
    session_id int auto_increment,
    session_code char(6) not null,
    quiz_id int,
    CONSTRAINT fk_session_quiz
    FOREIGN KEY(quiz_id) REFERENCES quiz(quiz_id) ON DELETE CASCADE ON UPDATE CASCADE,
    PRIMARY KEY(session_id)
);

CREATE TABLE participant(
    session_id int,
    participant_id int auto_increment,
    user_id int,
    CONSTRAINT fk_participant_session
    FOREIGN KEY(session_id) REFERENCES session(session_id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_participant_user
    FOREIGN KEY(user_id) REFERENCES user(user_id) ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY(participant_id)
);

-- left score
CREATE TABLE score(
    score_id int auto_increment,
    score_dt timestamp DEFAULT CURRENT_TIMESTAMP,
    score int not null,
    quiz_id int not null,
    character_id int not null,
    CONSTRAINT fk_score_quiz
    FOREIGN KEY(quiz_id) REFERENCES quiz(quiz_id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_score_character
    FOREIGN KEY(character_id) REFERENCES characterx(character_id) ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY(score_id)
);

CREATE TABLE response(
    response_id int,
    participant_id int,
    session_id int,
    response_ans varchar(200),
    is_correct char(1) DEFAULT 'N',
    response_dt timestamp DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_response_participant
    FOREIGN KEY(participant_id) REFERENCES participant(participant_id) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT fk_response_session
    FOREIGN KEY (session_id) REFERENCES session(session_id) ON UPDATE CASCADE ON DELETE CASCADE,
    PRIMARY KEY(response_id, participant_id, session_id)
);

-- populating user table
-- read code below

INSERT INTO user VALUES (null, 'admin', 'm7P5Gzv0XoWswnf8jNFUb4yfBnNju54NufYoZpRYS9E=', 'nKc/9hcaFsOytNiPqJmLEilfTX/X4r1R7JBLBOktYzA=', null, null, 0, 0, 3);
-- password1
INSERT INTO user VALUES (null, 'test1', '7zPDHZUMsamgTRVoyIhIVw1vyRabXKbbE4ZOnjPppPo=', '9aWgfl50G4QvcE3c+RbMTGgTuJq/Z4vsEfINKIa46Fc=', null, null, 0, 0, 0);
-- password2
INSERT INTO user VALUES (null, 'test2', 'SHZXDkhfZi+9+XL4iRp0NeWZzcoYo+W/xyNtXRed6WM=', '4sHMpk6dQD2ZMuLuT8IuY+NkPErqpmfjCJOBJQm2x28=', null, null, 0, 0, 0);
-- password3
INSERT INTO user VALUES (null, 'test3', 'hgnQgOjLS/V97N1LJxJf85pPDLJS70RNd2G+5CQd7xg=', 'VvnyuOS2++TXWgM4G3/MyJQEorNTlbO1kmO3GbvsrCc=', null, null, 0, 0, 0);
-- test4 password4
INSERT INTO user VALUES (null, 'test4', '0IGOtyvbdKdbhXi5kD8sRBvV/r0oX1zsIOaGM0cIFCo=', 'csBdlj06C+mJzpQ0pIbHUDK8aH5YLpJAveV5218yNC4=', null, null, 0, 0, 0);
-- test5 password5
INSERT INTO user VALUES (null, 'test5', 'CdbMRnXF3METNhkvTna0Cv9qKF1UZEE4NTC+eZlj6Fc=', 'hrG+P6BF8tiXKqVzuA32i1qsAIlHJWSk/4HMk375T/Y=', null, null, 0, 0, 0);
-- test6 password6
INSERT INTO user VALUES (null, 'test6', 'Cm4rVpEpS7v3Tc9AkH1SJGS8iP2+9CbMINOfA+mXeXI=', 'woWxOamsJW6o+bj8BULH+coyD6dZgfTnws4F9v2ofNw=', null, null, 0, 0, 0);
-- ophis z123456z
INSERT INTO user VALUES (null, 'ophis', 'mAhHiNVvtZMZuLmwgyYeNF16iMVKPKTBddyZFzAk3Ek=', 'w9IfHBHZadb/YIrNS0fZEBdCpeNvTON29cgNJnvzw/w=', null, null, 0, 0, 3);
-- howy0009 password8
INSERT INTO user VALUES (null, 'howy0009', 'AQFxsiwrKLWa/QF2RHlXvkJDfdBfZnDcZbE9ZsPW5qo=', 'XOvN6MWGYTtYCBYuxfF1SbJIgVY6FMnqeN2Jr660hD8=', null, null, 0, 0, 0);
-- djtfoo password9
INSERT INTO user VALUES (null, 'djtfoo', '0xFHVR2Bpb8iN2/obXGpn/wJCGLY+IovoqLMijLexuk=', '4/4DTgFB7jYBhPAFpT7glmUH4hI7zr/7Qoad9VC8UNg=', null, null, 0, 0, 0);
-- timisanillusion password10
INSERT INTO user VALUES (null, 'tim', 'dulctwGRuo0M6VI5OVRGoBaXktpvx/9T3IfoxiAYtjI=', 'hIC2TPV9ezQtFfGcN5sFMqDe+0Px2jupKjpnJDo81fc=', null, null, 0, 0, 0);
-- steffi password11
INSERT INTO user VALUES (null, 'steffi', '8IprqtMOE+suB3TtqYjbrUQzlXu6RDa2PUa/v8cEo+o=', 'r3CizqjuoMRVW2RJ2Mt3wq/oZPHzNhzX0h/twrvgp4Q=', null, null, 0, 0, 0);
-- tbhxx password12
INSERT INTO user VALUES (null, 'tbhxx', 'Q3rWsPHQRSyLXpqR4UVPNULBjgf7GeAebPboUf4qwrg=', 'Bhst1eSwheoMHzf5aZ87mc9PBGSAo9f4sd0Wy302byQ=', null, null, 0, 0, 0);
-- sayosauce 123456
INSERT INTO user VALUES (null, 'sayosauce', 'K0bHTgyIcivrZc0eHQ61bPnqN2jc1Zpx0igMBdjUdco=', '4oXZ+WkzHop+/oQLYP+pmysP0kr0HMn4gxX/ognj52c=', null, null, 0, 0, 0);
-- phantomcow 123456
INSERT INTO user VALUES (null, 'phantomcow', 'Wtquv6cKCHzVL4EfzX5r1M2bYC8PD7HPBpnOy+xJq6g=', 'YSIWZtYIv6w1MPy3DjN7ZgIS58PYVIF5Ou8Uiw1lSMY=', null, null, 0, 0, 0);
-- linyue 123456
INSERT INTO user VALUES (null, 'linyue', 'BZxnPWXXOUfR4ud4wLUm81OrinvACmOv0rEsLjTCnk8=', 'r2//yc4A1OYoh+Jr9sQwlcxPz23zKdqEAug/7ibX6ME=', null, null, 0, 0, 0);
-- shernaliu 123456
INSERT INTO user VALUES (null, 'shernaliu', 'lyuTm4zSdAN9tH3TJPA+gA+smzCURLwIIsCEZvPYkcg=', 'fls5dgKLhHbQXdvTwQNzxgsMlg2C1trWxzokRA7JtQE=', null, null, 0, 0, 0);
-- snsy 123456
INSERT INTO user VALUES (null, 'snsy', 'AfSJykbnhusVwfESREpzQQXvZ64Ys9/B1oaAbMeGUAk=', '9TB5xve7I4pcSBHz9HUlF0OetF9HBYeo26E5piH1Xcw=', null, null, 0, 0, 0);
-- shinnika 123456
INSERT INTO user VALUES (null, 'shinnika', '5F0I67LZLe9VPu9aKFrDL7zXW81vgC6ZWrW8HqEKwDY=', '7tL7Qb9afSrbFRfoqJa3UWQj1rW5+4dtGrTiGZK4TYY=', null, null, 0, 0, 0);
-- kwonsling 123456
INSERT INTO user VALUES (null, 'kwonsling', 'Er3Q8guCGMvgsfTg5CDB/bqFyo6XV3a2iN/3WMC3Mcw=', 'LJXBtTEilHlZ2tednzZsW4C/0kiW859aAKPtgFuk9hc=', null, null, 0, 0, 0);

INSERT INTO characterx VALUES (null, 1, "admin", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 2, "TestCharacter1", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 3, "TestCharacter2", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 4, "TestCharacter3", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 5, "TestCharacter4", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 6, "TestCharacter5", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 7, "TestCharacter6", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 8, "Ophis", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 9, "YiDe", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 10, "djtfoo", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 11, "Steffi", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 12, "TBHxx", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 13, "linyue", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 14, "shernaliu", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 15, "snsy", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 16, "Shinnika", 0, 0, 0, 0, 0, 0);
INSERT INTO characterx VALUES (null, 17, "kwonsling", 0, 0, 0, 0, 0, 0);