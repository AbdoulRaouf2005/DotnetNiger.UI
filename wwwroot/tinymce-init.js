function loadScript(src) {
  return new Promise((resolve, reject) => {
    if (document.querySelector(`script[src="${src}"]`)) {
      resolve();
      return;
    }
    const script = document.createElement('script');
    script.src = src;
    script.onload = resolve;
    script.onerror = () => reject(new Error(`Échec du chargement de ${src}`));
    document.head.appendChild(script);
  });
}

function loadCSS(href) {
  return new Promise((resolve) => {
    if (document.querySelector(`link[href="${href}"]`)) {
      resolve();
      return;
    }
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = href;
    link.onload = resolve;
    document.head.appendChild(link);
  });
}

window.loadTinyMCE = async (id, initialContent = '') => {
  if (typeof tinymce === 'undefined') {
    await Promise.all([
      loadScript('/lib/tinymce/tinymce.min.js'),
      loadCSS('/lib/tinymce/skins/ui/oxide/skin.min.css'),
      loadCSS('/lib/tinymce/skins/ui/oxide/content.min.css')
    ]);
  }

  const existing = tinymce.get(id);
  if (existing) {
    existing.destroy();
  }

  const element = document.getElementById(id);
  if (!element) {
    console.error(`[TinyMCE] Élément #${id} introuvable.`);
    return;
  }

  tinymce.init({
    selector: '#' + id,
    base_url: '/lib/tinymce',
    license_key: 'gpl',
    height: 380,
    menubar: true,
    plugins: 'lists link image table code',
    toolbar: 'undo redo | bold italic underline | alignleft aligncenter alignright | bullist numlist | link image | code',
    skin_url: '/lib/tinymce/skins/ui/oxide',
    content_css: '/lib/tinymce/skins/content/default/content.min.css',
    init_instance_callback: (editor) => {
      if (initialContent) {
        editor.setContent(initialContent);
      }
    },
    setup: (editor) => {
      editor.on('remove', () => {
        const ta = document.getElementById(id);
        if (ta) ta.value = '';
      });
    }
  });
};

window.getTinyMCEContent = (id) => {
  if (typeof tinymce === 'undefined') return '';
  const editor = tinymce.get(id);
  if (!editor) return '';
  return editor.getContent();
};

window.setTinyMCEContent = (id, content) => {
  if (typeof tinymce === 'undefined') return;
  const editor = tinymce.get(id);
  if (!editor) return;
  editor.setContent(content || '');
};

window.destroyTinyMCE = (id) => {
  if (typeof tinymce === 'undefined') return;
  const editor = tinymce.get(id);
  if (editor) {
    editor.destroy();
  }
};
