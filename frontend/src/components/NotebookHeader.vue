<script setup>
defineProps({
  title: {
    type: String,
    default: 'SEW AI Capstone - Overload'
  },
  subtitle: {
    type: String,
    default: 'ANALOG GYM JOURNAL // VOL. 1'
  },
  icon: {
    type: String,
    default: 'mdi-dumbbell'
  },
  currentUserId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['switch-user', 'logout'])

const onSwitchUser = () => {
  emit('switch-user')
  emit('logout')
}
</script>

<template>
  <v-app-bar
    flat
    elevation="0"
    class="notebook-app-bar"
    data-testid="notebook-app-bar"
  >
    <!-- Top binder tape strip -->
    <div class="binder-tape">
      <div class="binder-perforations"></div>
    </div>

    <!-- Paper Texture & Red Margin Accent -->
    <div class="header-inner d-flex align-center justify-space-between w-100 px-3 px-md-4">
      <div class="red-margin-indicator"></div>

      <!-- Left: Title / Branding -->
      <slot name="title">
        <div class="d-flex align-center header-brand">
          <div class="stamp-icon-box mr-3">
            <v-icon :icon="icon" size="small" class="stamp-icon"></v-icon>
          </div>
          <div class="d-flex flex-column justify-center">
            <span class="header-title font-weight-bold">{{ title }}</span>
            <span class="header-subtitle text-caption">{{ subtitle }}</span>
          </div>
        </div>
      </slot>

      <!-- Center / Default Content Slot -->
      <slot></slot>

      <!-- Right: User Tag & Actions -->
      <slot name="append">
        <div v-if="currentUserId" class="d-flex align-center user-controls">
          <div
            class="notebook-user-chip mr-2 d-flex align-center"
            data-testid="current-user-chip"
          >
            <span class="chip-stamp-label mr-1">USER:</span>
            <span class="chip-username font-weight-bold">{{ currentUserId }}</span>
          </div>

          <button
            type="button"
            class="notebook-btn notebook-btn--tactile"
            data-testid="btn-switch-user"
            @click="onSwitchUser"
          >
            <v-icon icon="mdi-account-switch" size="small" class="mr-1"></v-icon>
            <span>Switch User</span>
          </button>
        </div>
      </slot>
    </div>
  </v-app-bar>
</template>

<style lang="scss" scoped>
// SCSS Variables for Paper Notebook Aesthetic
$paper-surface: #f6ebd0;
$paper-bright: #fbf2d3;
$paper-lowest: #fcf5dc;
$ink-primary: #1f1d18;
$ink-container: #2c2820;
$pencil-graphite: #4a4639;
$rule-line: #cdbe8d;
$margin-red: #b91c1c;

.notebook-app-bar {
  background-color: $paper-surface !important;
  color: $ink-primary !important;
  border-bottom: 2px solid $ink-primary !important;
  box-shadow: none !important;
  overflow: visible;
  background-clip: padding-box;

  // Lined Paper Background Effect
  background-image: repeating-linear-gradient(
    to bottom,
    transparent,
    transparent 23px,
    rgba($rule-line, 0.4) 23px,
    rgba($rule-line, 0.4) 24px
  );

  // Top Dark Binder Strip (emulating spiral/bound legal pad tape)
  .binder-tape {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 5px;
    background-color: $ink-container;
    border-bottom: 1px solid $ink-primary;

    .binder-perforations {
      height: 100%;
      background: repeating-linear-gradient(
        90deg,
        rgba(255, 255, 255, 0.4) 0px,
        rgba(255, 255, 255, 0.4) 4px,
        transparent 4px,
        transparent 10px
      );
    }
  }

  .header-inner {
    height: 100%;
    position: relative;
  }

  // Classic Legal Pad Vertical Red Margin Line
  .red-margin-indicator {
    position: absolute;
    top: 5px;
    bottom: 0;
    left: 4px;
    width: 2px;
    background-color: rgba($margin-red, 0.65);
    pointer-events: none;
  }

  // Brand and Title Styling
  .header-brand {
    user-select: none;

    .stamp-icon-box {
      background-color: $paper-lowest;
      border: 1.5px solid $ink-primary;
      border-radius: 2px;
      padding: 4px 6px;
      display: flex;
      align-items: center;
      justify-content: center;
      box-shadow: 1.5px 1.5px 0 0 $ink-primary;
      transform: rotate(-1.5deg);
      transition: transform 0.15s ease;

      &:hover {
        transform: rotate(0deg) scale(1.05);
      }

      .stamp-icon {
        color: $ink-primary;
      }
    }

    .header-title {
      font-size: 1.25rem;
      letter-spacing: -0.01em;
      line-height: 1.1;
      color: $ink-primary;
      text-transform: uppercase;
    }

    .header-subtitle {
      font-size: 0.75rem;
      color: $pencil-graphite;
      letter-spacing: 0.05em;
      text-transform: uppercase;
    }
  }

  // User Badge / Chip
  .user-controls {
    gap: 8px;

    .notebook-user-chip {
      background-color: $paper-lowest;
      border: 1.5px solid $ink-primary;
      border-radius: 0;
      padding: 4px 10px;
      box-shadow: 1.5px 1.5px 0 0 $ink-primary;
      font-size: 0.85rem;
      color: $ink-primary;
      transform: rotate(0.5deg);

      .chip-stamp-label {
        font-size: 0.7rem;
        color: $pencil-graphite;
        letter-spacing: 0.05em;
      }

      .chip-username {
        color: $ink-primary;
      }
    }

    // Tactile Notebook Button
    .notebook-btn {
      background-color: $paper-bright;
      border: 1.5px solid $ink-primary;
      color: $ink-primary;
      font-size: 0.85rem;
      font-weight: 700;
      padding: 5px 12px;
      display: inline-flex;
      align-items: center;
      cursor: pointer;
      text-transform: uppercase;
      letter-spacing: 0.02em;
      outline: none;
      box-shadow: 2px 2px 0 0 $ink-primary;
      transition: background-color 0.1s ease, transform 0.08s ease, box-shadow 0.08s ease;

      &:hover {
        background-color: #ede2c7;
      }

      &--tactile:active {
        transform: translate(1.5px, 1.5px);
        box-shadow: 0 0 0 0 $ink-primary !important;
      }
    }
  }
}
</style>
