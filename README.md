HipChatConsole
==============

This is a simple console app that interacts with the HipChat api.  You can use it in interactive mode or headless with TeamCity (etc.).  It currently allows you to list rooms and send messages with a few options.

We have been using this in manual builds and other automated processes as a means to aggregate information into team rooms.  It is especially useful for long running processes to notify us as the process goes along.

#Usage

Below are the currently supported commands.

## Help

Shows the list of supported commands and how to use them

    ?

## Get rooms

Gets a list of rooms

    getrooms

## Send

Sends a message

    send from:CmBot notify:true roomid:123456 message:Hello world!
