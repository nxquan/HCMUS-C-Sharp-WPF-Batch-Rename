# Giới thiệu

***Họ tên*: Nguyễn Xuân Quân**

***MSSV*: 20120555**

***Email*: 20120555@student.hcmus.edu.vn**
**Original Description: ** https://www.notion.so/Project-batch-rename-2022-9dc9eb9c9d674dbdb4a988a3794d1335
# Đồ án: Batch Rename
***Hoàn thành:*** 
**A. Core requirements (7 points):**
1. Dynamically load all renaming rules from external DLL files
2. Select all files you want to rename
3. Create a set of rules for renaming the files. 
    1. Each rule can be added from a menu 
    2. Each rule's parameters can be edited 
4. Apply the set of rules in numerical order to each file, make them have a new name
5. Save this set of rules into presets for quickly loading later if you need to reuse

**B. Improvements (3 points):**
1. Drag & Drop a file to add to the list
2. Storing parameters for renaming using JSON file
3. Handling duplication: 
   - Xử lý trùng khi thêm file và khi đổi tên ta sẽ thêm thứ tự cho tên bị trùng
4. Autosave & load the current working condition to prevent sudden power loss
   1. The current file list
   2. The current set of renaming rules, together with the parameters
5. Let the user see the preview of the result
6. Create a copy of all the files and move them to a selected folder rather than perform the renaming on the original file
7. Drag & Drop a rule to change the order of the list rules

***Khó khăn:***
- Drag and drop trong một danh sách (ListView) mới đầu muốn thay đổi real-time nhưng không được

***Điểm mong muốn:*** 10 điểm ạ!
