<?php
require "vendor/autoload.php";
use Abraham\TwitterOAuth\TwitterOAuth;

// Twitter API credentials
$consumerKey = 'YOUR_CONSUMER_KEY';
$consumerSecret = 'YOUR_CONSUMER_SECRET';
$accessToken = 'YOUR_ACCESS_TOKEN';
$accessTokenSecret = 'YOUR_ACCESS_TOKEN_SECRET';

// Initialize TwitterOAuth instance
$twitter = new TwitterOAuth($consumerKey, $consumerSecret, $accessToken, $accessTokenSecret);

// Path to the image to be uploaded
$imagePath = $_FILES['screenshot']['tmp_name'];

// Upload the media to Twitter
$media = $twitter->upload('media/upload', ['media' => $imagePath]);

// Check if the media upload was successful
if (!isset($media->media_id_string)) {
    echo "Error uploading media to Twitter.";
    exit;
}

// Post a tweet with the uploaded media
$status = $twitter->post('statuses/update', [
    'status' => 'Check out my score in Tower It Up!',
    'media_ids' => $media->media_id_string
]);

// Check for errors
if ($twitter->getLastHttpCode() == 200) {
    echo "Tweet posted successfully.";
} else {
    echo "Error posting tweet: " . $status->errors[0]->message;
}
