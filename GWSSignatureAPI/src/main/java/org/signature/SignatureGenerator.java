package org.signature;

import com.google.api.client.auth.oauth2.*;
import com.google.api.client.googleapis.auth.oauth2.*;
import com.google.api.client.googleapis.javanet.GoogleNetHttpTransport;
import com.google.api.client.http.*;
import com.google.api.client.json.JsonFactory;
import com.google.api.client.json.jackson2.JacksonFactory;
import com.google.api.services.oauth2.*;
import com.google.api.services.oauth2.model.Userinfo;

import java.io.IOException;
import java.security.GeneralSecurityException;
import java.util.Collections;
import java.util.Scanner;

public class SignatureGenerator {
    private static final String CLIENT_ID = "276055439798-0t06p7t9ojtqo98p5eo6v0ngh3n5lb5e.apps.googleusercontent.com";
    private static final String CLIENT_SECRET = "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";
    private static final String REDIRECT_URI = "http://localhost:8080/callback";
    private static final String SCOPES = "https://www.googleapis.com/auth/userinfo.profile";

    public static void main(String[] args) throws IOException, GeneralSecurityException {
        JsonFactory jsonFactory = JacksonFactory.getDefaultInstance();
        HttpTransport httpTransport = GoogleNetHttpTransport.newTrustedTransport();

        GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow.Builder(
                httpTransport, jsonFactory, CLIENT_ID, CLIENT_SECRET, Collections.singleton(SCOPES))
                .setAccessType("offline")
                .build();

        String authorizationUrl = flow.newAuthorizationUrl().setRedirectUri(REDIRECT_URI).build();
        System.out.println("Please open the following URL in your browser:");
        System.out.println(authorizationUrl);

        // Prompt user to enter the authorization code from the browser
        Scanner scanner = new Scanner(System.in);
        System.out.print("Enter the authorization code: ");
        String authorizationCode = scanner.nextLine();
        scanner.close();

        GoogleTokenResponse tokenResponse = flow.newTokenRequest(authorizationCode).setRedirectUri(REDIRECT_URI).execute();
        Credential credential = flow.createAndStoreCredential(tokenResponse, null);

        Oauth2 userInfoService = new Oauth2.Builder(httpTransport, jsonFactory, credential).build();
        Userinfo userInfo = userInfoService.userinfo().get().execute();

        System.out.println("Profile Information:");
        System.out.println("ID: " + userInfo.getId());
        System.out.println("Name: " + userInfo.getName());
        System.out.println("Email: " + userInfo.getEmail());
        System.out.println("Picture URL: " + userInfo.getPicture());
    }
}
