//Nav Dropdown list
function DropDownList() {
  let dropDownIcon = document.getElementById("dropdown-icon");
  let dropDownList = document.getElementById("dropdown-list");
  dropDownIcon.addEventListener("click", (event) => {
    event.stopPropagation(); // Prevents click from propagating to document
    dropDownList.classList.toggle("d-none");
  });
  document.addEventListener("click", (event) => {
    if (
      !dropDownIcon.contains(event.target) &&
      !dropDownList.contains(event.target)
    ) {
      dropDownList.classList.add("d-none");
      dropDownList.classList.remove("d-flex");
    }
  });
}
DropDownList();
