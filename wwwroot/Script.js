window.closeMenuOnOutsideClick = (dotnetHelper) => {
      document.addEventListener("click",function(event){
            dotnetHelper.invokeMethodAsync("CloseMenu")
      })
}

window.initAvatarDropZone = (dropZoneId, inputId) => {
      const dropZone = document.getElementById(dropZoneId);
      const input = document.getElementById(inputId);

      if (!dropZone || !input || dropZone.dataset.avatarDropzoneInitialized === "true") {
            return;
      }

      dropZone.dataset.avatarDropzoneInitialized = "true";

      const setActiveState = (isActive) => {
            dropZone.classList.toggle("border-indigo-400", isActive);
            dropZone.classList.toggle("bg-indigo-50", isActive);
            dropZone.classList.toggle("ring-2", isActive);
            dropZone.classList.toggle("ring-indigo-200", isActive);
      };

      dropZone.addEventListener("dragenter", (event) => {
            event.preventDefault();
            setActiveState(true);
      });

      dropZone.addEventListener("dragover", (event) => {
            event.preventDefault();
            event.dataTransfer.dropEffect = "copy";
            setActiveState(true);
      });

      dropZone.addEventListener("dragleave", (event) => {
            if (event.target === dropZone) {
                  setActiveState(false);
            }
      });

      dropZone.addEventListener("drop", (event) => {
            event.preventDefault();
            setActiveState(false);

            const file = event.dataTransfer?.files?.[0];
            if (!file) {
                  return;
            }

            const dataTransfer = new DataTransfer();
            dataTransfer.items.add(file);
            input.files = dataTransfer.files;
            input.dispatchEvent(new Event("change", { bubbles: true }));
      });
};