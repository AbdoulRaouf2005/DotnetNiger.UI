/**
 * tinymce-init.js
 *
 * Gestion robuste de TinyMCE pour Blazor WASM / SPA.
 *
 * Problèmes résolus :
 *  1. Validation "field required" : on expose destroyTinyMCE + le contenu
 *     est récupéré AVANT la soumission via getContent(), jamais via une
 *     variable C# non synchronisée.
 *  2. Contenu non chargé en mode édition : setContent est appelé dans
 *     init_instance_callback, APRÈS que l'éditeur est vraiment prêt.
 *  3. Éditeur qui disparaît : on détruit proprement l'instance avant que
 *     Blazor re-rende le composant (via destroyTinyMCE appelé dans Dispose).
 */

// ── Utilitaire : attend que TinyMCE soit disponible dans window ──────────────

function waitForTinyMCE(callback, timeout = 8000) {
  const start = Date.now();
  const poll = () => {
    if (typeof tinymce !== 'undefined') {
      callback();
    } else if (Date.now() - start < timeout) {
      setTimeout(poll, 100);
    } else {
      console.error('[TinyMCE] Timeout : TinyMCE ne s\'est pas chargé.');
    }
  };
  poll();
}

// ── initTinyMCE(id, initialContent) ─────────────────────────────────────────
//
//  - Détruit toute instance existante sur cet id (navigation SPA safe)
//  - Initialise TinyMCE
//  - Injecte initialContent une fois l'éditeur prêt
//
window.initTinyMCE = (id, initialContent = '') => {
  waitForTinyMCE(() => {
    // Détruire l'instance précédente si elle existe (retour sur la page)
    const existing = tinymce.get(id);
    if (existing) {
      existing.destroy();
    }

    const element = document.getElementById(id);
    if (!element) {
      console.error(`[TinyMCE] Élément #${id} introuvable dans le DOM.`);
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

      // ► Injecter le contenu ici, quand l'éditeur est vraiment prêt.
      //   C'est le seul endroit fiable — pas dans OnAfterRenderAsync.
      init_instance_callback: (editor) => {
        if (initialContent) {
          editor.setContent(initialContent);
        }
      },

      // Empêche TinyMCE de vider le textarea original lors du destroy
      // (évite des warnings Blazor sur des nodes DOM modifiés)
      setup: (editor) => {
        editor.on('remove', () => {
          const ta = document.getElementById(id);
          if (ta) ta.value = '';
        });
      }
    });
  });
};

// ── getTinyMCEContent(id) ────────────────────────────────────────────────────
//
//  Retourne le HTML de l'éditeur, ou '' si non trouvé.
//  À appeler côté Blazor juste avant la soumission du formulaire.
//
window.getTinyMCEContent = (id) => {
  if (typeof tinymce === 'undefined') {
    console.warn('[TinyMCE] Non chargé, contenu vide retourné.');
    return '';
  }
  const editor = tinymce.get(id);
  if (!editor) {
    console.warn(`[TinyMCE] Éditeur #${id} non trouvé.`);
    return '';
  }
  return editor.getContent();
};

// ── setTinyMCEContent(id, content) ──────────────────────────────────────────
//
//  Utilisé uniquement si on veut mettre à jour le contenu APRÈS
//  l'initialisation (ex. rechargement dynamique). Dans le cas normal,
//  passer initialContent à initTinyMCE est suffisant.
//
window.setTinyMCEContent = (id, content) => {
  if (typeof tinymce === 'undefined') return;
  const editor = tinymce.get(id);
  if (!editor) return;
  editor.setContent(content || '');
};

// ── destroyTinyMCE(id) ───────────────────────────────────────────────────────
//
//  À appeler depuis IAsyncDisposable.DisposeAsync() de chaque composant
//  qui héberge un éditeur. Cela évite les "editor zombie" lors de la
//  navigation SPA et le bug "l'éditeur disparaît".
//
window.destroyTinyMCE = (id) => {
  if (typeof tinymce === 'undefined') return;
  const editor = tinymce.get(id);
  if (editor) {
    editor.destroy();
  }
};
