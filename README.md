# Lorem-Ipsum-UCD23
COMP47250 Summer 2023 Team Software Project - Lorem Ipsum


### Dev Flow
1. Create a branch with a descriptive name
   - (GOOD) image-detection
   - (BAD) aaa, sample
2. Add/Update features
   - In the main branch, type ```git branch -b [branch name]```
   - Constantly merge the main branch to your branch.
3. Check if it passes all the unit tests and works as expected on HoloLens
4. Create a PR
   - Make sure to assign reviewers and assignees to speed things up.
5. After getting an approval, merge it to the main branch
   - You can do it from the PR on Github
6. Delete the working branch on your local
   - It avoids confusion.
   - type ```git delete -d [branch name]```. If it causes an error, highly likey either your local git is not up-to-date or your branch is not merged yet.
 


### Requrements
- Unity 2021.3.26f
- HoloLens 2
