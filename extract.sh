#!/bin/bash
#hg list | while read line; do mkdir -p ~/git/stratotune/$line; done;
hg list | while read line; do (cd $line && hg archive -t files ~/git/stratotune/$line); done;

