// 初始化打开页面的类型
function saveUrl() {

	var params = {}
	var localObj = window.location
	var urls = localObj.href.split("?")
	var htmlPath = urls[0].split("/")
	var pageType = htmlPath[htmlPath.length - 1].replace(".html", "")

	params = {
		"pageType": pageType
	}


	if (pageType === "goods-detail") {
		return;
	}


	window.localStorage.setItem("page", JSON.stringify(params))
}

saveUrl()



function jump(id) {

	window.localStorage.setItem("goodId", id);
	window.location.href = "./detail.html?id=" + id;
}

// 拦截导航栏跳转链接
function navEven(url) {
	// saveUrl()
	window.location.href = url
}


// 根据数组的中的ID获取商品
function getGoods(data, id) {

	var len = data.length
	for (var i = 0; i < len; i++) {
		// console.log(data[i])
		if (data[i].id === id) {
			return data[i];
		}
	}
}

// 初始化可选的商品参数
function initAttrData(tagId, goodsData) {
	var attrTag = document.getElementById(tagId);
	var attrs = goodsData.attrs;

	var attrLen = attrs == "" || typeof(attrs) == "undefined" ? 0 : attrs.length
	for (var i = 0; i < attrLen; i++) {
		var key = attrs[i].key;
		var value = attrs[i].value;

		var e_0 = document.createElement("li");
		var e_1 = document.createElement("span");
		var div = document.createElement("div")
		e_1.setAttribute("style", "float: left;");
		e_1.appendChild(document.createTextNode(key + "："));

		var num = 0;
		var valLen = value.length;
		for (var j = 0; j < valLen; j++) {
			num++;
			var btn = document.createElement("button");
			btn.innerHTML = value[j]
			btn.className = "attrBtn"
			div.appendChild(btn)
		}
		div.id = "attr-choose"
		e_1.style.height = (num * 28) + "px"
		e_0.appendChild(e_1);
		e_0.appendChild(div)
		attrTag.appendChild(e_0);

		// 获取到参数的高度，同时设置标签的高度
		// console.log(window.getComputedStyle(document.getElementById("attr-choose")).getPropertyValue("height"))
		// console.log(window.getComputedStyle(document.getElementById("attr-choose")).getPropertyValue("height"))
		var h = window.getComputedStyle(document.getElementById("attr-choose")).getPropertyValue("height")
		e_1.style.height = h


	}
}

// 初始化参数规格面板内容
function initGuigePanel(good, idbyDiv) {
	var guige = good.guige
	var guigeLen = guige == "" || typeof(guige) == "undefined" ? 0 : guige.length
	var ul = document.createElement("ul");
	for (var i = 0; i < guigeLen; i++) {
		var li = document.createElement("li");
		var span = document.createElement("span")
		span.innerHTML = guige[i].key + "："
		span.style.letterSpacing = "3px"
		li.appendChild(span)
		li.append(guige[i].value)
		ul.appendChild(li)
	}
	document.getElementById(idbyDiv).append(ul);
}


// tag切换面板
function jumpTab(name, obj) {
	// 初始化选中的tag页
	var item = document.getElementById(name);
	var parentNodes = item.parentNode.childNodes;
	var parentLen = parentNodes.length
	for (var i = 0; i < parentLen; i++) {
		var thisClassName = parentNodes[i].className
		// console.log(parentNodes[i])
		// 获取所有子节点会包括注释等标签，用判断方式判断这个标签是否又类名，有说明他是一个dom对象
		if (thisClassName != null && thisClassName != "") {
			parentNodes[i].style.display = 'none';
			// console.log(parentNodes[i].className)
		}
	}
	item.style.display = 'block';

	// 初始化tag导航的样式
	var tagNavNodes = obj.parentNode.parentNode.childNodes // 获取LI标签
	var tagNavLen = tagNavNodes.length
	for (var i = 0; i < tagNavLen; i++) {
		if (tagNavNodes[i].tagName === "LI" || tagNavNodes[i].tagName === "li") {
			// 因为子节点可能会有多个a标签，我们只需要获取第一个即可
			var nowObj = tagNavNodes[i].getElementsByTagName("a")[0];
			nowObj.style.color = "";
			nowObj.style.backgroundColor = "";
		}
	}

	obj.style.color = "white";
	obj.style.backgroundColor = "#69c0ff";
}

// 检查图片地址是否正常: 参考地址：https://blog.csdn.net/qq_38543537/article/details/108391483
function checkImgExists(imgUrl) {
	var imgObj = new Image();

	imgObj.onerror = function() {
		return false;
	}
	imgObj.src = imgUrl;
	// 存在
	if (imgObj.fileSize > 0 || (imgObj.width > 0 && imgObj.height > 0)) {
		console.log('图片地址有效')
		return true;
	} else {
		console.log('图片地址无效')
		return false;
	}
}

// 参考：https://www.cnblogs.com/mfrbuaa/p/4255349.html
// 获取绝对路径 spliceIndex:路径切割的起点  spliceNum： 路径切割的数量
function getAbsolutePath(spliceIndex, spliceNum) {
	var localObj = window.location
	var contextPath = localObj.href.split("/")
	contextPath.splice((contextPath.length - spliceIndex), spliceNum)
	var basePath = contextPath.join("/")
	return basePath;
}

// 保存路径上的参数值
function setParam() {

	var params = {}
	var localObj = window.location
	var urls = localObj.href.split("?")

	// 获取参数的字符串部分
	var paramPrefix = urls[1]
	// console.log(paramPrefix)
	paramPrefix = paramPrefix.split("&")
	var len = paramPrefix.length


	for (var i = 0; i < len; i++) {
		var l = paramPrefix[i].split("=")
		key = l[0]
		value = l[1]
		params[key] = value
	}

	window.localStorage.setItem("params", JSON.stringify(params))
}

function getParam(key) {
	var data = window.localStorage.getItem(key)
	return JSON.parse(data);
}


// 复制对象
function copyProperties(data) {
	return JSON.parse(JSON.stringify(data));
}

function getPageInfo(data, index, limit) {
	var data = copyProperties(data)
	return data.splice(index, limit)
}

// 初始化div数据
function initPanelData(obj, data) {

	// 获取到项目的绝对路径，保证图片文件能正常使用
	var path = getAbsolutePath(2, 2) + "/img/detail-img/"
	var objImgs = obj.getElementsByTagName("img")
	var objLink = obj.getElementsByTagName("a")
	var objTitle = obj.getElementsByClassName("goods-title")
	var objILen = objImgs.length
	for (var i = 0; i < objILen; i++) {
		// console.log(data[i].pic)
		// console.log(path + data[i].prefixPath + data[i].pic)
		// console.log(data)
		objImgs[i].src = path + data[i].prefixPath + "/" + data[i].pic
		objImgs[i].width = "185px"
		objImgs[i].style.height = "210px"

		objTitle[i].innerHTML = data[i].title

		// 设置跳转链接事件
		objLink[i].setAttribute("onclick", "jump('" + data[i].id + "')")
		obj.getElementsByClassName("goods-title")[i].style.color = "#595959"
		obj.getElementsByClassName("goods-title")[i].style.marginTop = "12px"
	}
}




function ideaForm() {
	document.getElementById('question').style.display = "block"
}



function ideaFormClose() {
	document.getElementById('question').style.display = "none"
}

function initHeaderColor() {
	var headerbody = document.getElementById('header-nav')
	var page = getParam('page').pageType

	if (typeof(page) == "undefined") {
		return;
	}

	var dom = page + "-navitem";

	var domObj = document.getElementById(dom)
	console.log(domObj)
	if (typeof(domObj) === "undefined") {
		return;
	}

	var domListByA = domObj.childNodes
	var len = domListByA.length
	for (var i = 0; i < len; i++) {
		console.log(domListByA[i])
		if (domListByA[i].tagName === "A") {
			domListByA[i].style.color = "#69c0ff"
		}
	}

}


// 动态渲染页面标题
function initMetaTitle() {
	var pageType = getParam("page").pageType
	console.log("------------------------")
	console.log(pageType)
	var data = []
	var prefix = "云梦商城-";
	if (pageType === "index") {
		document.title = prefix + "首页"
	} else if (pageType === "clothing-market") {
		document.title = prefix + "服装市场"
	} else if (pageType === "phone-market") {
		document.title = prefix + "智能手机"
	} else if (pageType === "toiletris-market") {
		document.title = prefix + "洗护用品"
	} else if (pageType === "food-market") {
		document.title = prefix + "食品生鲜"
	} else if (pageType === "household-market") {
		document.title = prefix + "精品家居"
	} else if (pageType === "book-market") {
		document.title = prefix + "图书音像"
	} else if (pageType === "about-us") {
		document.title = prefix + "关于我们"
	} else if (pageType === "user-info") {
		document.title = prefix + "我的主页"
	}

}


initHeaderColor();
initMetaTitle();

// 导入js数据文件
// function addScript(url){
//     var script = document.createElement('script');
//     script.setAttribute('type','text/javascript');
//     script.setAttribute('src',url);
//     document.getElementsByTagName('head')[0].appendChild(script);
// }
