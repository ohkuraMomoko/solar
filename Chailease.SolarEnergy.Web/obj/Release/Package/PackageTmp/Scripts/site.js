
	/* **************************************** *
	* GLOBAL
	* **************************************** */
	// save gnb submenu dropdown background size
	var gnb_sub_menu_height = 0;

	// search popup open check (small_screen_nav_open / search_popup)
	var search_open = true;

	/* **************************************** *
	 * FUNCTIONS
	 * **************************************** */
	// FIX HEADER ANIMATION
	function minimize_header() {
		var $window = $(window);
		var $header = $("#header");
		var didScroll = null;
		var currentScroll = 0;
		var lastScroll = 0;
		var moveScroll = 10;

		$window.on("scroll", function () {
			didScroll = true;

			if ($window.scrollTop() > $header.height()) {
				$header.addClass("minimize");
			} else {
				$header.removeClass("minimize");
			}
		});

		if (!$header.hasClass("jt_none_scrolled")) {
			setInterval(function () {
				if (didScroll) {
					hasScrolled();
					didScroll = false;
				}
			}, 50);
		}

		function hasScrolled() {
			currentScroll = $(this).scrollTop();

			// Make sure they scroll more than moveScroll
			if (Math.abs(lastScroll - currentScroll) <= moveScroll) return;

			if (currentScroll > lastScroll) {
				// ScrollDown
				if (currentScroll > $(window).height()) {
					TweenMax.to($header, 0.4, {
						autoAlpha: 0,
						y: -$header.outerHeight(),
						ease: Power3.easeOut
					});
				}
			} else {
				// ScrollUp
				TweenMax.to($header, 0.4, {
					autoAlpha: 1,
					y: 0,
					ease: Power3.easeOut
				});
			}

			lastScroll = currentScroll;
		}
	}

	// SMALL SCREEN NAV
	function small_screen_nav_open() {
		var $body = $("body"),
			$menu_btn = $("#small_menu_btn"),
			$menu_close_btn = $("#small_menu_close"),
			$menu_overlay = $("#small_menu_overlay"),
			$menu_container = $("#small_menu_container"),
			$lang_menu_container = $(".js-lang_container"),
			$active_menu = null,
			isOpen = true;

		// menu on/off
		$menu_btn.on("click", function () {
			if (isOpen) {
				$body.addClass("open_menu");

				// active menu check
				if ($body.hasClass("single-notice")) {
					$active_menu = $("#small_menu > li.menu-item-promote");
				} else if (
					$body.hasClass("single-career") ||
					$body.hasClass("single-inquiry")
				) {
					$active_menu = $("#small_menu > li.menu-item-career");
				} else {
					$("#small_menu > li").each(function () {
						if ($(this).hasClass("current-page-ancestor")) {
							$active_menu = $(this);
						}
					});
				}

				if ($active_menu !== null) {
					$active_menu.addClass("open");
					$("#small_depth_menu_nav").addClass("open");
					TweenLite.set($("#small_depth_menu_nav"), { x: "-100%" });
					TweenLite.to(
						$("#small_depth_menu").find(
							".parent-" + $active_menu.attr("id")
						),
						0.3,
						{ autoAlpha: 1 }
					);
				}

				// open
				TweenLite.to($menu_overlay, 0.3, {
					autoAlpha: 1,
					onStart: function () {
						$menu_overlay.css("display", "block");
					}
				});
				TweenLite.fromTo(
					$menu_container,
					0.3,
					{ x: "0%" },
					{
						x: "-100%",
						onStart: function () {
							$menu_container.css("display", "block");
							TweenLite.to(
								[$lang_menu_container],
								0.3,
								{ autoAlpha: 1, delay: 0.2 }
							);
						},
						onComplete: function () {
							isOpen = false;
						}
					}
				);
				TweenLite.to($menu_close_btn, 0.5, {
					x: -(
						$menu_container.outerWidth() +
						$menu_close_btn.outerWidth() +
						5
					)
				});
			} else {
				menu_close_action();
			}
		});

		// close menu
		$("#small_menu_close, #small_menu_overlay").on("click", function () {
			menu_close_action();
		});

		// close menu
		function menu_close_action() {
			TweenLite.to($menu_overlay, 0.5, {
				autoAlpha: 0,
				onComplete: function () {
					$menu_overlay.css("display", "none");
				}
			});
			TweenLite.to($menu_container, 0.5, {
				x: "100%",
				onStart: function () {
					TweenLite.set([$lang_menu_container], {
						autoAlpha: 0
					});

					// search close
					if (!search_open) {
						$("#search_popup").css("display", "none"); // hide motion
						search_close_action();
					}
				},
				onComplete: function () {
					$menu_container.css("display", "none");

					$("#small_menu > li").removeClass("open");
					$("#small_depth_menu_nav").removeClass("open");
					TweenLite.set($("#small_depth_menu_nav"), { x: "0%" });
					TweenLite.set($("#small_depth_menu > li"), {
						autoAlpha: 0
					});

					$body.removeClass("open_menu");
					isOpen = true;
				}
			});
			TweenLite.to($menu_close_btn, 0.3, { x: 0 });
		}
	}

	// 2depth menu
	function small_screen_nav_toggle() {
		// menu top level link
		$("#small_menu").on("click", "li", function (e) {
			e.preventDefault();

			var $this = $(this);
			var depth_class = ".parent-" + $this.attr("id");
			var $depth_menu = $("#small_depth_menu").find(depth_class);

			if ($depth_menu.length > 0) {
				// 1depth
				$("#small_menu > li").removeClass("open");
				$this.addClass("open");

				// 2depth
				if (!$("#small_depth_menu_nav").hasClass("open")) {
					$("#small_depth_menu_nav").addClass("open");

					TweenLite.fromTo(
						$("#small_depth_menu_nav"),
						0.3,
						{
							x: "0%"
						},
						{
							x: "-100%",
							onStart: function () {
								TweenLite.set($("#small_depth_menu > li"), {
									autoAlpha: 0
								});
								TweenLite.to($depth_menu, 0.3, {
									autoAlpha: 1
								});
							}
						}
					);
				} else {
					TweenLite.set($("#small_depth_menu > li"), {
						autoAlpha: 0
					});
					TweenLite.to($depth_menu, 0.3, { autoAlpha: 1 });
				}
			} else {
				window.location.href = $(this)
					.find("a")
					.attr("href");
			}
		});
	}

	// GNB MENU HOVER
	function screen_nav_hover() {
		$("#menu").hover(
			function () {
				TweenMax.killTweensOf($(".menu_container_bg"));

				TweenMax.to($(".menu_container_bg"), 0.3, {
					height: gnb_sub_menu_height,
					onStart: function () {
						TweenMax.set($(".menu_container_bg"), { autoAlpha: 1 });
						$("#menu ul.sub-menu")
							.stop()
							.slideDown(300);
					}
				});
			},
			function () {
				TweenMax.killTweensOf($(".menu_container_bg"));

				TweenMax.to($(".menu_container_bg"), 0.3, {
					height: 0,
					onStart: function () {
						$("#menu ul.sub-menu")
							.stop()
							.slideUp(280);
					},
					onComplete: function () {
						TweenMax.set($(".menu_container_bg"), { autoAlpha: 0 });
					}
				});
			}
		);

		$("#menu")
			.on("focusin", function () {
				TweenMax.killTweensOf($(".menu_container_bg"));

				TweenMax.to($(".menu_container_bg"), 0.3, {
					height: gnb_sub_menu_height,
					onStart: function () {
						TweenMax.set($(".menu_container_bg"), { autoAlpha: 1 });
						$("#menu ul.sub-menu")
							.stop()
							.slideDown(300);
					}
				});
			})
			.on("focusout", function () {
				TweenMax.killTweensOf($(".menu_container_bg"));

				TweenMax.to($(".menu_container_bg"), 0.3, {
					height: 0,
					onStart: function () {
						$("#menu ul.sub-menu")
							.stop()
							.slideUp(280);
					},
					onComplete: function () {
						TweenMax.set($(".menu_container_bg"), { autoAlpha: 0 });
					}
				});
			});
	}

	// GNB layout setting
	function menu_layout_setting() {
		// add background markup (full menu)
		$(".menu_container").prepend('<span class="menu_container_bg"></span>');

		// add small menu markup
		$("#menu > li").each(function () {
			var $this = $(this),
				$sub_menu = $this.find("> ul.sub-menu"),
				parent_id = "parent-" + $this.attr("id");

			// 1depth
			$("#small_menu").append(
				$this
					.clone()
					.find("> ul.sub-menu")
					.remove()
					.end()
			);

			// 2depth
			if (!$sub_menu.length) {
				var $sub_menu_li = '<li class="' + parent_id + '"></li>';
				var $sub_menu_clone = $sub_menu.clone();

				$("#small_depth_menu").append($sub_menu_li);
				$("." + parent_id).append($sub_menu_clone);
			}
		});
	}

	// GNB 2depth menu match height
	function menu_height_setting() {
		var $sub_menu = $("#menu > li > ul.sub-menu");

		// clear
		gnb_sub_menu_height = 0;
		$sub_menu.height("auto");

		// sub menu height matching
		$sub_menu.each(function () {
			if ($(this).height() > gnb_sub_menu_height) {
				gnb_sub_menu_height = $(this).height();
			}
		});
		$sub_menu.height(gnb_sub_menu_height);
	}

	// scroll top button
	function scroll_top() {
		var $window = $(window);
		var $document = $(document);
		var $footer = $("#footer");
		var $scrollBtn = $("#go_top");

		$scrollBtn.on("click", function () {
			$("html, body")
				.stop()
				.animate(
					{
						scrollTop: 0
					},
					600
				);

			return false;
		});

		$window.on("scroll", function () {
			if (
				$window.scrollTop() <
				$document.height() -
				$window.height() -
				$footer.outerHeight() +
				37
			) {
				$scrollBtn.addClass("js_go_top_fix");
			} else {
				$scrollBtn.removeClass("js_go_top_fix");
			}

			if ($window.scrollTop() < $window.height()) {
				$scrollBtn.addClass("js_go_top_hide");
			} else {
				$scrollBtn.removeClass("js_go_top_hide");
			}
		});
	}

	// scroll down button
	function scroll_down() {
		$(".scroll_down").on("click", function () {
			var target = $(this).attr("href");
			var target_top = $(target).offset().top;
			var header_height = $("#header").height();
			var space = 0;

			if (!$("#header").hasClass("minimize")) {
				if (!is_screen(1023)) {
					space = 15;
				} else if (!is_screen(768)) {
					space = 10;
				}
			}

			$("html,body").animate(
				{
					scrollTop: target_top - header_height + space
				},
				600
			);

			return false;
		});
	}

$(function(){
	//init---------------
	// menu_layout_setting();
	menu_height_setting();
	screen_nav_hover();
	small_screen_nav_open();
	small_screen_nav_toggle();
	minimize_header();

	var cachedWidth = $(window).width();
	$(window).resize(function () {
		var newWidth = $(window).width();
		if (newWidth !== cachedWidth) {
			//避免在手機scroll就會觸發resize event
			cachedWidth = newWidth;
			// if (detectmob() || $(window).width() <= 1024) {
			// 	location.reload();
			// }
			menu_height_setting();
			screen_nav_hover();
			small_screen_nav_open();
			small_screen_nav_toggle();
			minimize_header();
		}
	});

});
