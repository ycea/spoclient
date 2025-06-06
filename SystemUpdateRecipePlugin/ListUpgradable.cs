﻿namespace SpoClient.Plugin.Recipe.V1.SystemUpdate
{
    class ListUpgradable : ISimpleRecipe
    {
        public string Name => "アップデートリスト";

        public string Description => "アップデート可能なパッケージの一覧を取得します";

        public string Command => "apt list --upgradable";

        public bool UseSudo => false;
    }
}
